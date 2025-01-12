import React, { useEffect, useState } from 'react'
import List from '@mui/material/List'
import ListItem from '@mui/material/ListItem'

interface StockAmount {
    name: string
    value: number
    price: number
    totalValue: number
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
    }, [])

    if (loading) {
        return <h2 style={{ color: '#FFD369' }}>Loading stocks...</h2>
    }

    return (
        <div style={{ padding: '20px', fontFamily: 'Arial, sans-serif' }}>
            <h2>Your Stocks</h2>
            <table
                style={{
                    width: '100%',
                    borderCollapse: 'collapse',
                    marginTop: '20px'
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
                            Price
                        </th>
                        <th
                            style={{
                                padding: '10px',
                                border: '1px solid #ddd'
                            }}
                        >
                            Total Value
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
