import React, { useEffect, useState } from 'react'
import '../App.css'

interface StockAmount {
    name: string
    value: number
    price: number
    totalValue: number
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

async function fetchUserStocks(): Promise<StockAmount[]> {
    try {
        const userId = sessionStorage.getItem('userId')
        const response = await fetch(
            `http://api.localhost/users/${userId}/stock`,
            {
                method: 'GET',
                headers: {
                    Authorization: `Bearer ${sessionStorage.getItem('token')}`
                }
            }
        )
        if (!response.ok) {
            throw new Error('Failed to fetch user stocks')
        }
        const data = await response.json()
        return data.stocksAmounts.map((item: any) => ({
            name: item.name,
            value: item.value,
            price: item.price,
            totalValue: item.totalValue
        }))
    } catch (error: any) {
        console.error('Error fetching user stock:', error.message)
        throw new Error(error.message || 'Unknown error occurred')
    }
}

const Stock = () => {
    const [userBalance, setUserBalance] = useState<number>(0)
    const [stocks, setStocks] = useState<StockAmount[]>([])
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        const getStocks = async () => {
            try {
                const fetchedStocks = await fetchUserStocks()
                const sortedStocks = fetchedStocks.sort(
                    (a, b) => b.value - a.value
                )
                setStocks(sortedStocks)
            } catch (err: any) {
                console.error('Error fetching stocks:', err.message)
            } finally {
                setLoading(false)
            }
        }

        getStocks()
        fetchUserBalance().then(balance => setUserBalance(balance))
    }, [])

    if (loading) {
        return <h2 style={{ color: '#FFD369' }}>Loading stocks...</h2>
    }

    return (
        <div
            style={{
                paddingLeft: '10px',
                paddingRight: '10px',
                fontFamily: 'Arial, sans-serif'
            }}
        >
            <div className="select-group">
                <div className="select-item-last">
                    <p className="select-label">
                        Balance: ${userBalance.toFixed(2)}
                    </p>
                </div>
            </div>
            <h2>Your Stocks</h2>
            <table
                style={{
                    maxWidth: '800px',
                    width: '100%',
                    borderCollapse: 'collapse',
                    marginTop: '20px',
                    marginLeft: 'auto',
                    marginRight: 'auto'
                }}
            >
                <thead>
                    <tr
                        style={{
                            backgroundColor: '#f4f4f4',
                            textAlign: 'left'
                        }}
                    >
                        <th
                            style={{
                                padding: '10px',
                                border: '1px solid #ddd'
                            }}
                        >
                            Ticker
                        </th>
                        <th
                            style={{
                                padding: '10px',
                                border: '1px solid #ddd'
                            }}
                        >
                            Stock Amount
                        </th>
                        <th
                            style={{
                                padding: '10px',
                                border: '1px solid #ddd'
                            }}
                        >
                            Current price
                        </th>
                        <th
                            style={{
                                padding: '10px',
                                border: '1px solid #ddd'
                            }}
                        >
                            Total Value: $
                            {stocks
                                .reduce(
                                    (acc, stock) => acc + stock.totalValue,
                                    0
                                )
                                .toFixed(2)}
                        </th>
                    </tr>
                </thead>
                <tbody>
                    {stocks.map(stock => (
                        <tr
                            key={stock.name}
                            style={{ borderBottom: '1px solid #ddd' }}
                        >
                            <td
                                style={{
                                    padding: '10px',
                                    border: '1px solid #ddd'
                                }}
                            >
                                {stock.name}
                            </td>
                            <td
                                style={{
                                    padding: '10px',
                                    border: '1px solid #ddd'
                                }}
                            >
                                {stock.value}
                            </td>
                            <td
                                style={{
                                    padding: '10px',
                                    border: '1px solid #ddd'
                                }}
                            >
                                ${stock.price.toFixed(2)}
                            </td>
                            <td
                                style={{
                                    padding: '10px',
                                    border: '1px solid #ddd'
                                }}
                            >
                                ${stock.totalValue.toFixed(2)}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    )
}

export default Stock
