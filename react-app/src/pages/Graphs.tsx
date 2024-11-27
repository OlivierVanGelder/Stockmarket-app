import React, { useState, useEffect, useMemo, useCallback } from 'react'
import '../App.css'
import LineChart from '../components/LineChart'
import CandleStickChart from '../components/CandleStickChart'
import ToggleButtonNotEmpty from '../components/ToggleButton'
import Interval from '../Interval'
import Popup from '../components/Popup'
import { Button } from '@mui/material'
import { styled } from '@mui/material/styles'

const BuyButton = styled(Button)(({ theme }) => ({
    backgroundColor: '#16fa4f',
    color: '#FFFF',
    '&:hover': {
        backgroundColor: '#6df78f'
    }
})) as typeof Button
const SellButton = styled(Button)(({ theme }) => ({
    backgroundColor: '#fa1616',
    color: '#FFFF',
    '&:hover': {
        backgroundColor: '#f76d6d'
    }
})) as typeof Button

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

async function fetchUserBalance(): Promise<number> {
    try {
        const userId = sessionStorage.getItem('userId')
        const response = await fetch(
            'https://localhost:42069/accounts/user/balance',
            {
                method: 'POST',
                headers: {
                    'Access-Control-Allow-Origin': '*',
                    'Content-Type': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify(userId)
            }
        )
        if (!response.ok) throw new Error('Failed to fetch user balance')
        const data = await response.json()
        return await data.userBalance
    } catch (error) {
        console.error('Error fetching user balance:', error)
        return 0
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
    const [startTimeString, setStartDay] = useState<string>('month')
    const [popupOpen, setPopupOpen] = useState(false)
    const [isBuy, setIsBuy] = useState(true)
    const [stockPrice, setStockPrice] = useState(100)
    const [chartData, setUserData] = useState<{
        labels: string[]
        datasets: any[]
    }>({
        labels: [],
        datasets: []
    })
    const [stockNames, setStockNames] = useState<string[]>([])
    const [userBalance, setUserBalance] = useState<number>(0)
    const [intervalOptions, setIntervalOptions] = useState<Interval[]>([
        new Interval(1, '1 day')
    ])
    const socket = useMemo(
        () => new WebSocket(`wss://localhost:42069/StockWS`),
        []
    )

    useEffect(() => {
        fetchStockNames().then(setStockNames)
        fetchUserBalance().then(setUserBalance)
    }, [])

    const openPopup = () => setPopupOpen(true)
    const closePopup = () => setPopupOpen(false)

    const handleInvest = (amount: number) => {
        if (isBuy) {
            // Logic for buying stock: usually, this would involve a backend call.
            alert(`You bought ${amount} stocks of ${ticker}.`)
            setUserBalance(userBalance - stockPrice * amount)
            closePopup() // Close modal after purchase
        } else {
            // Logic for selling stock: usually, this would involve a backend call.
            setUserBalance(userBalance + stockPrice * amount)
            closePopup()
        }
    }

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
            console.log('Received data:', event.data)
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
                <div className="select-item">
                    <div
                        style={{
                            marginTop: 'auto',
                            marginBottom: '10px'
                        }}
                    >
                        <BuyButton
                            onClick={() => {
                                setIsBuy(true)
                                openPopup()
                            }}
                            size="large"
                        >
                            Buy
                        </BuyButton>
                    </div>
                </div>
                <div className="select-item">
                    <div
                        style={{
                            marginTop: 'auto',
                            marginBottom: '10px'
                        }}
                    >
                        <SellButton
                            onClick={() => {
                                setIsBuy(false)
                                openPopup()
                            }}
                            size="large"
                        >
                            Sell
                        </SellButton>
                    </div>
                </div>
                <div className="select-item-last">
                    <p className="select-label">Balance: {userBalance}</p>
                </div>
            </div>
            <div>
                <Popup
                    isBuy={isBuy}
                    isOpen={popupOpen}
                    onClose={closePopup}
                    onSubmit={handleInvest}
                    stockPrice={stockPrice}
                    userBalance={userBalance}
                />
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
