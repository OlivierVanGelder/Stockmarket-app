import React, { useState, useEffect, useMemo, useCallback } from 'react'
import '../App.css'
import LineChart from '../components/LineChart'
import CandleStickChart, {
    CandleGraphItem
} from '../components/CandleStickChart'
import ToggleButtonNotEmpty from '../components/ToggleButton'
import Interval from '../Interval'
import Popup from '../components/Popup'
import { Button } from '@mui/material'
import { styled } from '@mui/material/styles'

const BuyButton = styled(Button)(() => ({
    backgroundColor: '#16fa4f',
    color: '#FFFF',
    '&:hover': {
        backgroundColor: '#6df78f'
    }
})) as typeof Button
const SellButton = styled(Button)(() => ({
    backgroundColor: '#fa1616',
    color: '#FFFF',
    '&:hover': {
        backgroundColor: '#f76d6d'
    }
})) as typeof Button

async function fetchStockNames(): Promise<string[]> {
    try {
        const response = await fetch('http://api.localhost/stocks/names')
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
            `http://api.localhost/users/${userId}/balance`,
            {
                method: 'GET',
                headers: {
                    Authorization: `Bearer ${sessionStorage.getItem('token')}`
                }
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

interface LineDataItem {
    Date: string
    Value: number
}

function Graphs() {
    const [candleSelected, setCandleSelected] = useState<boolean>(false)
    const [invalidData, setInvalidData] = useState<boolean>(false)
    const [ticker, setTicker] = useState<string>('APPL')
    const [candleData, setCandleData] = useState<CandleGraphItem[]>([])
    const [interval, setInterval] = useState<number>(1 / 24)
    const [startTimeString, setStartDay] = useState<string>('month')
    const [popupOpen, setPopupOpen] = useState(false)
    const [isBuy, setIsBuy] = useState(true)
    const [stockPrice, setStockPrice] = useState(100)
    const [stockAmount, setStockAmount] = useState(0)
    const [chartData, setUserData] = useState<{
        labels: string[]
        datasets: {
            label: string
            values: number[]
            backgroundColor: string
            borderColor: string
        }[]
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
        () => new WebSocket(`ws://api.localhost/stocks/StockWS`),
        []
    )

    useEffect(() => {
        fetchStockNames().then(setStockNames)
        fetchUserBalance().then(setUserBalance)
    }, [])

    const openPopup = () => {
        setPopupOpen(true)
    }
    const closePopup = () => setPopupOpen(false)

    async function handleInvest(amount: number): Promise<boolean> {
        if (invalidData) {
            alert('Cannot trade with invalid data')
            return false
        }

        const userId = sessionStorage.getItem('userId')
        const response = await fetch(
            `http://api.localhost/users/${userId}/stock`,
            {
                method: 'PUT',
                headers: {
                    Authorization: `Bearer ${sessionStorage.getItem('token')}`,
                    'Content-Type': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify({
                    amount,
                    ticker,
                    Price: stockPrice,
                    Action: isBuy ? 'buy' : 'sell'
                })
            }
        )

        if (!response.ok) {
            return false
        }

        const data = await response.json()
        const success: boolean = data.success
        if (!success) {
            return false
        }
        if (isBuy) {
            setUserBalance(userBalance - stockPrice * amount)
        } else {
            setUserBalance(userBalance + stockPrice * amount)
        }
        closePopup()
        return true
    }

    useEffect(() => {
        const userId = sessionStorage.getItem('userId')
        fetch(
            `http://api.localhost/users/${userId}/stock/amount?ticker=${ticker}`,
            {
                method: 'GET',
                headers: {
                    Authorization: `Bearer ${sessionStorage.getItem('token')}`
                }
            }
        )
            .then(response => response.json())
            .then(data => {
                setStockAmount(data)
            })
    }, [userBalance, ticker])

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
            const newData = JSON.parse(event.data) as
                | LineDataItem[]
                | CandleGraphItem[]
            if (candleSelected) {
                applyCandleData(newData as CandleGraphItem[])
            } else {
                applyLineData(ticker, newData as LineDataItem[])
            }
        }
    }, [interval, startTimeString, candleSelected, ticker])

    const applyLineData = useCallback(
        (ticker: string, data: LineDataItem[]) => {
            const labels = data.map(item => item.Date)
            if (labels.length != 0) {
                setInvalidData(false)
                const values = data.map(item => item.Value)
                setStockPrice(data[data.length - 1].Value)
                setUserData({
                    labels,
                    datasets: [
                        {
                            label: `Stock Price for ${ticker}`,
                            values,
                            backgroundColor: 'rgb(242, 139, 130)',
                            borderColor: 'rgb(242, 139, 130)'
                        }
                    ]
                })
            } else {
                setInvalidData(true)
            }
        },
        []
    )

    const applyCandleData = useCallback((data: CandleGraphItem[]) => {
        if (!Array.isArray(data) || !data.length) {
            setInvalidData(true)
            console.error('Invalid candle data received:', data)
            return
        }
        setInvalidData(false)
        console.log('Candle data:', data)
        setStockPrice(data[data.length - 1].Close)
        setCandleData(data)
    }, [])

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
                        className="cypress-start-time-select"
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
                            className="cypress-buy-button"
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
                            className="cypress-sell-button"
                        >
                            Sell
                        </SellButton>
                    </div>
                </div>
                <div className="select-item-last">
                    <p className="select-label">
                        Balance: {userBalance.toFixed(2)}
                    </p>
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
            <h2 className="cypress-stockAmount">Stock owned: {stockAmount}</h2>
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
                            <LineChart
                                chartData={chartData.datasets}
                                labels={chartData.labels}
                            />
                        </div>
                    )}
                </div>
            )}
        </div>
    )
}

export default Graphs
