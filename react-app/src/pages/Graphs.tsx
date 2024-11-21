import React, { useState, useEffect, useMemo, useCallback } from 'react'
import '../App.css'
import LineChart from '../components/LineChart'
import CandleStickChart from '../components/CandleStickChart'
import ToggleButtonNotEmpty from '../components/ToggleButton'
import Interval from '../Interval'

async function fetchStockNames(): Promise<string[]> {
    try {
        const response = await fetch('https://localhost:42069/stocks/names')
        if (!response.ok) throw new Error('Failed to fetch stock names')
        return await response.json()
    } catch (error) {
        console.error('Error fetching stock names:', error)
        return []
    }
}

function convertToDays(date: Date): number {
    const referenceDate = new Date('2020-11-01T12:00:00Z')
    return parseFloat(
        (
            (date.getTime() - referenceDate.getTime()) /
            (1000 * 60 * 60 * 24)
        ).toFixed(10)
    )
}

interface CandleDataItem {
    x: Date
    y: [number, number, number, number]
    volume: number
}

function Graphs() {
    const [candleSelected, setCandleSelected] = useState<boolean>(false)
    const [invalidData, setInvalidData] = useState<boolean>(false)
    const [ticker, setTicker] = useState<string>('APPL')
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
    const [stockNames, setStockNames] = useState<string[]>([])
    const [intervalOptions, setIntervalOptions] = useState<Interval[]>([
        new Interval(7, '1 week')
    ])
    const socket = useMemo(
        () => new WebSocket(`wss://localhost:42069/StockWS`),
        []
    )

    // Fetch stock names when the component mounts
    useEffect(() => {
        fetchStockNames().then(setStockNames)
    }, [])

    useEffect(() => {
        const options: Record<string, Interval[]> = {
            hour: [new Interval(0.0006944444444, '1 min')],
            hours: [
                new Interval(0.0006944444444, '1 min'),
                new Interval(0.033333333333, '30 min'),
                new Interval(0.04166666667, '1 hour')
            ],
            day: [
                new Interval(0.033333333333, '30 min'),
                new Interval(0.04166666667, '1 hour')
            ],
            week: [
                new Interval(0.04166666667, '1 hour'),
                new Interval(0.5, '12 hours'),
                new Interval(1, '1 day')
            ],
            month: [
                new Interval(0.04166666667, '1 hour'),
                new Interval(0.5, '12 hours'),
                new Interval(1, '1 day')
            ],
            year: [new Interval(1, '1 day'), new Interval(7, '1 week')],
            all: [new Interval(30, '30 days'), new Interval(365, '1 year')]
        }
        const newOptions = options[startTimeString] || []
        setIntervalOptions(newOptions)
        if (
            newOptions.length &&
            !newOptions.some(opt => opt.value === interval)
        ) {
            const newInterval = newOptions[newOptions.length - 1].value
            setInterval(newInterval)
            return
        }

        const sendMessage = () => {
            const currentDate = new Date()
            switch (startTimeString) {
                case 'hour':
                    currentDate.setHours(currentDate.getHours() - 1)
                    break
                case 'hours':
                    currentDate.setHours(currentDate.getHours() - 6)
                    break
                case 'day':
                    currentDate.setDate(currentDate.getDate() - 1)
                    break
                case 'week':
                    currentDate.setDate(currentDate.getDate() - 7)
                    break
                case 'month':
                    currentDate.setMonth(currentDate.getMonth() - 1)
                    break
                case 'year':
                    currentDate.setFullYear(currentDate.getFullYear() - 1)
                    break
                case 'all':
                    currentDate.setFullYear(currentDate.getFullYear() - 100)
                    break
            }
            let startTime = convertToDays(currentDate)
            startTime = Math.max(startTime, 0)

            const endDay = convertToDays(new Date())
            if ((endDay - startTime) / interval > 1000) {
                setInterval(intervalOptions[intervalOptions.length - 1].value)
            }
            const message = candleSelected
                ? `${ticker}-${interval}-${startTime}-${endDay}-candle`
                : `${ticker}-${interval}-${startTime}-${endDay}`

            socket.send(JSON.stringify(message))
        }

        if (socket.readyState === WebSocket.OPEN) {
            sendMessage()
        } else {
            socket.onopen = sendMessage
        }

        socket.onmessage = event => {
            const newData = JSON.parse(event.data)
            if (candleSelected) {
                applyCandleData(newData)
            } else {
                applyLineData(ticker, newData)
            }
        }
    }, [interval, startTimeString, candleSelected, ticker])

    const applyLineData = useCallback((ticker: string, data: any) => {
        const labels = data.map((item: any) => item.Date)
        if (labels.length != 0) {
            setInvalidData(false)
            data = data.map((item: any) => item.Value)
            setUserData({
                labels,
                datasets: [
                    {
                        label: `Stock Price for ${ticker}`,
                        data,
                        backgroundColor: 'rgb(242, 139, 130)',
                        borderColor: 'rgb(242, 139, 130)'
                    }
                ]
            })
        } else {
            setInvalidData(true)
        }
    }, [])

    const applyCandleData = useCallback((data: any) => {
        if (!Array.isArray(data) || !data.length) {
            setInvalidData(true)
            console.error('Invalid candle data received:', data)
            return
        }
        setInvalidData(false)
        const formattedData: CandleDataItem[] = data.map((item: any) => ({
            x: new Date(item.Date),
            y: [item.Open, item.High, item.Low, item.Close],
            volume: item.Volume
        }))
        setCandleData(formattedData)
    }, [])

    // Render options outside of JSX for readability
    const startTimeOptions = [
        'hour',
        'hours',
        'day',
        'week',
        'month',
        'year',
        'all'
    ].map(option => (
        <option
            key={option}
            value={option}
        >{`${option[0].toUpperCase()}${option.slice(1)}`}</option>
    ))

    return (
        <div>
            <div className="select-group">
                <div className="select-item">
                    <p className="select-label">Stock:</p>
                    <select
                        id="stock-select"
                        value={ticker}
                        onChange={e => setTicker(e.target.value)}
                    >
                        {stockNames.map((name, index) => (
                            <option key={index} value={name}>
                                {name}
                            </option>
                        ))}
                    </select>
                </div>

                <div className="select-item">
                    <p className="select-label">Start Time:</p>
                    <select
                        id="start-time-select"
                        value={startTimeString}
                        onChange={e => setStartDay(e.target.value)}
                    >
                        {startTimeOptions}
                    </select>
                </div>

                <div className="select-item">
                    <p className="select-label">Interval:</p>
                    <select
                        id="interval-select"
                        value={interval}
                        onChange={e => setInterval(parseFloat(e.target.value))}
                    >
                        {intervalOptions.map((opt, index) => (
                            <option key={index} value={opt.value}>
                                {opt.element}
                            </option>
                        ))}
                    </select>
                </div>
            </div>

            <ToggleButtonNotEmpty
                candleSelected={candleSelected}
                setCandleSelected={setCandleSelected}
            />
            {invalidData ? (
                <div>
                    <h1>Invalid Data</h1>
                </div>
            ) : (
                <div>
                    {candleSelected ? (
                        <CandleStickChart dataset={candleData} />
                    ) : (
                        <div
                            style={{
                                width: 1200,
                                backgroundColor: '#FFFFFF',
                                margin: '35px'
                            }}
                        >
                            <LineChart chartData={chartData} />
                        </div>
                    )}
                </div>
            )}
        </div>
    )
}

export default Graphs
