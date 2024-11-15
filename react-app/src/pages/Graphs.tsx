import React, { useState, useEffect, useRef, useMemo } from 'react'
import '../App.css'
import LineChart from '../components/LineChart'
import CandleStickChart from '../components/CandleStickChart'
import ToggleButtonNotEmpty from '../components/ToggleButton'
import Interval from '../Interval'
import { start } from 'repl'

async function fetchStockNames(): Promise<string[]> {
    try {
        const response = await fetch('https://localhost:42069/stocknames')
        if (!response.ok) {
            throw new Error('Failed to fetch stock names')
        }
        return await response.json()
    } catch (error) {
        console.error('Error fetching stock names:', error)
        return []
    }
}

function convertToDays(date: Date): number {
    const referenceDate = new Date('2020-11-01T12:00:00Z')

    const diffInMs = date.getTime() - referenceDate.getTime()

    // Convert the difference from milliseconds to days
    const diffInDays = diffInMs / (1000 * 60 * 60 * 24) // 1000 ms * 60 s * 60 min * 24 hrs

    return parseFloat(diffInDays.toFixed(10))
}

function Graphs() {
    const [candleSelected, setCandleSelected] = useState<boolean>(false)
    const [ticker, setTicker] = useState<string>('AAPL')
    const [candleData, setCandleData] = useState<CandleDataItem[]>([])
    const [interval, setInterval] = useState<number>(1 / 24)
    const [startTimeString, setStartDay] = useState<string>('all')
    const [chartData, setUserData] = useState<{
        labels: string[]
        datasets: any[]
    }>({
        labels: [],
        datasets: []
    })
    const [stocknames, setStockNames] = useState<string[]>([])
    const [intervalOptions, setIntervalOptions] = useState<Interval[]>([
        new Interval(7, '1 week')
    ])
    const socket = useMemo(
        () => new WebSocket(`wss://localhost:42069/StockWS`),
        []
    )

    useEffect(() => {
        const fetchData = async () => {
            setStockNames(await fetchStockNames())
        }
        fetchData()
    }, [])

    useEffect(() => {
        let newIntervalOptions: Interval[] = []
        switch (startTimeString) {
            case 'hour':
                newIntervalOptions = [new Interval(0.0006944444444, '1 min')]
                break
            case 'hours':
                newIntervalOptions = [
                    new Interval(0.0006944444444, '1 min'),
                    new Interval(0.033333333333, '30 min'),
                    new Interval(0.04166666667, '1 hour')
                ]
                break
            case 'day':
                newIntervalOptions = [
                    new Interval(0.033333333333, '30 min'),
                    new Interval(0.04166666667, '1 hour')
                ]
                break
            case 'week':
                newIntervalOptions = [
                    new Interval(0.04166666667, '1 hour'),
                    new Interval(0.5, '12 hours'),
                    new Interval(1, '1 day')
                ]
                break
            case 'month':
                newIntervalOptions = [
                    new Interval(0.04166666667, '1 hour'),
                    new Interval(0.5, '12 hours'),
                    new Interval(1, '1 day')
                ]
                break
            case 'year':
                newIntervalOptions = [
                    new Interval(1, '1 day'),
                    new Interval(7, '1 week')
                ]
                break
            case 'all':
                newIntervalOptions = [new Interval(7, '1 week')]
                break
        }

        setIntervalOptions(newIntervalOptions)
        if (newIntervalOptions.length > 0) {
            setInterval(newIntervalOptions[newIntervalOptions.length - 1].value)
        }
    }, [startTimeString])

    useEffect(() => {
        const sendMessage = () => {
            const currentDate = new Date()
            let startTime: number = 0
            switch (
                startTimeString
                // Same switch cases as before...
            ) {
            }
            startTime = convertToDays(currentDate)
            if (startTime < 0) {
                startTime = 0
            }
            const endDay = convertToDays(new Date())
            if ((endDay - startTime) / interval > 1000) {
                setInterval(intervalOptions[intervalOptions.length - 1].value)
            }
            if (candleSelected) {
                socket.send(
                    JSON.stringify(
                        `${ticker}-${interval}-${startTime}-${endDay}-candle`
                    )
                )
                console.log(
                    `${ticker}-${interval}-${startTime}-${endDay}-candle`
                )
                console.log(currentDate)
            } else {
                socket.send(
                    JSON.stringify(
                        `${ticker}-${interval}-${startTime}-${endDay}`
                    )
                )
            }
        }

        if (socket.readyState === socket.OPEN) sendMessage()
        else socket.onopen = sendMessage

        socket.onmessage = event => {
            const newData = JSON.parse(event.data)
            console.log('Received data:', newData)
            if (candleSelected) {
                ApplyCandleData(setCandleData, newData)
            } else {
                applyLineData(
                    ticker,
                    setUserData,
                    ['rgb(242, 139, 130)'],
                    newData
                )
            }
        }
    }, [interval, startTimeString, candleSelected, ticker])

    async function applyLineData(
        ticker: string,
        setUserData: React.Dispatch<
            React.SetStateAction<{ labels: string[]; datasets: any[] }>
        >,
        color: string[],
        newStockData: any
    ) {
        if (newStockData) {
            const extractedData = newStockData
                .map((item: any) => item.date)
                .toString()
                .slice(0, 16)
            setUserData({
                labels: extractedData,
                datasets: [
                    {
                        label: `Stock Price for ${ticker}`,
                        data: newStockData,
                        backgroundColor: color,
                        borderColor: color
                    }
                ]
            })
        }
    }

    interface CandleDataItem {
        x: Date
        y: [open: number, high: number, low: number, close: number]
        volume: number
    }

    async function ApplyCandleData(
        setCandleData: React.Dispatch<React.SetStateAction<CandleDataItem[]>>,
        newCandleData: any
    ) {
        // Check if newCandleData is an array and has valid data
        if (!Array.isArray(newCandleData) || newCandleData.length === 0) {
            console.error('Invalid candle data received:', newCandleData)
            return // Exit if data is invalid
        }

        const candleData = newCandleData.reduce<CandleDataItem[]>(
            (acc, item, index) => {
                if (item && typeof item === 'object') {
                    acc.push({
                        x: item.Date, // Use the corresponding date
                        y: [item.Open, item.High, item.Low, item.Close],
                        volume: item.Volume
                    })
                } else {
                    console.error('Invalid candle item format:', item)
                }
                return acc
            },
            []
        ) // Start with an empty array
        setCandleData(candleData)
    }

    return (
        <div>
            <select
                value={ticker}
                onChange={e => {
                    const value: string = e.target.value.toString()
                    setTicker(value)
                }}
            >
                {stocknames.map((element, index) => (
                    <option key={index} value={element}>
                        {element}
                    </option>
                ))}
            </select>

            <select
                value={startTimeString}
                onChange={e => {
                    const value: string = e.target.value
                    setStartDay(value)
                }}
            >
                <option value={'hour'}>{'1 hour'}</option>
                <option value={'hours'}>{'6 hour'}</option>
                <option value={'day'}>{'1 day'}</option>
                <option value={'week'}>{'1 week'}</option>
                <option value={'month'}>{'1 month'}</option>
                <option value={'year'}>{'1 year'}</option>
                <option value={'all'}>{'all'}</option>
            </select>

            <select
                value={interval}
                onChange={e => {
                    const value: number = parseFloat(e.target.value)
                    setInterval(value)
                }}
            >
                {intervalOptions.map((interval, index) => (
                    <option key={index} value={interval.value}>
                        {interval.element}
                    </option>
                ))}
            </select>
            <ToggleButtonNotEmpty
                candleSelected={candleSelected}
                setCandleSelected={setCandleSelected}
            />
            {!candleSelected ? (
                <div
                    style={{
                        width: 1200,
                        backgroundColor: '#FFFFFF',
                        margin: '35px'
                    }}
                >
                    <LineChart chartData={chartData} />
                </div>
            ) : (
                <CandleStickChart dataset={candleData} />
            )}
        </div>
    )
}

export default Graphs
