import React, { useEffect, useState } from 'react'
import List from '@mui/material/List'
import ListItem from '@mui/material/ListItem'

interface StockAmount {
    name: string
    value: number
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
            value: item.value
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
            <List
                sx={{
                    width: '100%',
                    maxWidth: 360,
                    bgcolor: 'background.paper'
                }}
            >
                {stocks.map((stock, index) => (
                    <ListItem key={index} disableGutters>
                        <h3 style={{ fontWeight: 'bold', margin: 0 }}>
                            {stock.name}:{stock.value.toFixed(2)}
                        </h3>
                    </ListItem>
                ))}
            </List>
        </div>
    )
}

export default Stock
