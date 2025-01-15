import {
    Card,
    Button,
    Dialog,
    DialogActions,
    DialogContent,
    DialogContentText,
    DialogTitle
} from '@mui/material'
import React, { useState, useEffect } from 'react'
import { Navigate, redirect, useNavigate } from 'react-router-dom'

async function deleteAccount(): Promise<boolean> {
    try {
        const userId = sessionStorage.getItem('userId')

        const response = await fetch(`http://api.localhost/users/${userId}`, {
            method: 'DELETE',
            headers: {
                Authorization: `Bearer ${sessionStorage.getItem('token')}`
            }
        })

        if (!response.ok) {
            return false
        }
        return true
    } catch (error) {
        console.error('Error deleting user:', error)
        return false
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
        if (!response.ok) throw new Error('Failed to fetch user name')
        const data = await response.json()
        return await data.userName
    } catch (error) {
        console.error('Error fetching user name:', error)
        return ''
    }
}

const Account = () => {
    const [userBalance, setUserBalance] = useState<number>(0.0)
    const [userName, setUserName] = useState<string>('')
    const [openDialog, setOpenDialog] = useState<boolean>(false)

    const navigate = useNavigate()
    const navigateToLogin = async () => {
        navigate('/login')
    }

    useEffect(() => {
        fetchUserName().then(setUserName)
        fetchUserBalance().then(setUserBalance)
    }, [])

    const handleDeleteAccount = async () => {
        const success = await deleteAccount()
        if (success) {
            alert('Account deleted successfully.')
            navigateToLogin()
        } else {
            alert('Failed to delete account.')
        }
        setOpenDialog(false)
    }

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
                    <Button
                        className="cypress-delete-account"
                        variant="outlined"
                        color="error"
                        onClick={() => setOpenDialog(true)}
                    >
                        Delete account
                    </Button>
                </Card>
            </div>

            {/* Confirmation Dialog */}
            <Dialog
                open={openDialog}
                onClose={() => setOpenDialog(false)}
                aria-labelledby="delete-confirmation-dialog"
            >
                <DialogTitle id="delete-confirmation-dialog">
                    Confirm Deletion
                </DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Are you sure you want to delete your account? This
                        action is irreversible.
                    </DialogContentText>
                </DialogContent>
                <DialogActions>
                    <Button
                        onClick={() => setOpenDialog(false)}
                        color="primary"
                    >
                        Cancel
                    </Button>
                    <Button
                        onClick={handleDeleteAccount}
                        color="error"
                        className="cypress-dialog-delete-account"
                    >
                        Delete
                    </Button>
                </DialogActions>
            </Dialog>
        </div>
    )
}

export default Account
