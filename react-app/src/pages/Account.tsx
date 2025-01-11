import { Card } from '@mui/material'
import React from 'react'
import { useState } from 'react'
import { useEffect } from 'react'

async function fetchUserBalance(): Promise<number> {
    try {
        const userId = sessionStorage.getItem('userId')
        const response = await fetch(
            `http://api.localhost/users/${userId}/balance`,
            {
                method: 'GET',
                headers: {
                    Authorization: `Bearer ${sessionStorage.getItem('token')}`
                },
                credentials: 'include'
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

async function fetchUserName(): Promise<string> {
    try {
        const userId = sessionStorage.getItem('userId')
        const response = await fetch(
            `http://api.localhost/users/${userId}/name`,
            {
                method: 'GET',
                headers: {
                    Authorization: `Bearer ${sessionStorage.getItem('token')}`
                },
                credentials: 'include'
            }
        )
        if (!response.ok) throw new Error('Failed to fetch user balance')
        const data = await response.json()
        return await data.userName
    } catch (error) {
        console.error('Error fetching user balance:', error)
        return ''
    }
}

const Account = () => {
    const [userBalance, setUserBalance] = useState<number>(0.0)
    const [userName, setUserName] = useState<string>('')

    useEffect(() => {
        fetchUserName().then(setUserName)
        fetchUserBalance().then(setUserBalance)
    }, [])

    return (
        <div>
            <div>
                <Card
                    style={{
                        width: 'fit-content',
                        padding: '80px',
                        paddingTop: '10px',
                        paddingBottom: '20px',
                        margin: 'auto',
                        boxSizing: 'border-box'
                    }}
                >
                    <h1>Account</h1>
                    <h3>Username: {userName}</h3>
                    <h3>Balance: ${userBalance.toFixed(2)}</h3>
                </Card>
            </div>
        </div>
    )
}

export default Account
