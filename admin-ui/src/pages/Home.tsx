import React, { useEffect, useState } from 'react'
import { Button } from '@mui/material'

interface User {
    id: string
    userName: string
    balance: number
}

async function deleteUser(userId: string): Promise<boolean> {
    try {
        const adminId = sessionStorage.getItem('userId')

        const response = await fetch(
            `http://api.localhost/users/${userId}?adminId=${adminId}`,
            {
                method: 'DELETE',
                headers: {
                    Authorization: `Bearer ${sessionStorage.getItem('token')}`
                }
            }
        )

        if (!response.ok) {
            return false
        }

        fetchAllUsers()
        return true
    } catch (error) {
        console.error('Error deleting user:', error)
        return false
    }
}

async function fetchAllUsers(): Promise<User[]> {
    try {
        const userId = sessionStorage.getItem('userId')

        const response = await fetch(
            `http://api.localhost/users?userId=${userId}`,
            {
                method: 'GET',
                headers: {
                    Authorization: `Bearer ${sessionStorage.getItem('token')}`
                }
            }
        )

        if (!response.ok) throw new Error('Failed to fetch users')

        const data = await response.json()
        return data.map((item: User) => ({
            id: item.id,
            userName: item.userName,
            balance: item.balance / 100
        }))
    } catch (error) {
        console.error('Error fetching users:', error)
        return []
    }
}

const Home = () => {
    const [users, setUsers] = useState<User[]>([])
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        const getUsers = async () => {
            try {
                const fetchedStocks = await fetchAllUsers()
                const sortedUsers = fetchedStocks.sort(
                    (a, b) => b.balance - a.balance
                )
                setUsers(sortedUsers)
            } catch (err: any) {
                console.error('Error fetching users:', err.message)
            } finally {
                setLoading(false)
            }
        }

        getUsers()
    }, [])

    if (loading) {
        return <h2 style={{ color: '#FFD369' }}>Loading users...</h2>
    }

    return (
        <div style={{ padding: '20px', fontFamily: 'Arial, sans-serif' }}>
            <h2>Users</h2>
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
                            Userid
                        </th>
                        <th
                            style={{
                                padding: '10px',
                                border: '1px solid #ddd'
                            }}
                        >
                            Username
                        </th>
                        <th
                            style={{
                                padding: '10px',
                                border: '1px solid #ddd'
                            }}
                        >
                            Balance
                        </th>
                        <th
                            style={{
                                padding: '10px',
                                border: '1px solid #ddd'
                            }}
                        ></th>
                    </tr>
                </thead>
                <tbody>
                    {users.map(user => (
                        <tr
                            key={user.id}
                            style={{ borderBottom: '1px solid #ddd' }}
                        >
                            <td
                                style={{
                                    padding: '10px',
                                    border: '1px solid #ddd'
                                }}
                            >
                                {user.id}
                            </td>
                            <td
                                style={{
                                    padding: '10px',
                                    border: '1px solid #ddd'
                                }}
                            >
                                {user.userName}
                            </td>
                            <td
                                style={{
                                    padding: '10px',
                                    border: '1px solid #ddd'
                                }}
                            >
                                ${user.balance.toFixed(2)}
                            </td>
                            <td
                                style={{
                                    padding: '10px',
                                    border: '1px solid #ddd'
                                }}
                            >
                                <Button
                                    variant="outlined"
                                    color="error"
                                    onClick={() => {
                                        deleteUser(user.id)
                                    }}
                                >
                                    Delete
                                </Button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    )
}

export default Home
