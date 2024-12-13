import React from 'react'
import { useState } from 'react'
import LoginForm from '../components/LoginForm'
import RegisterForm from '../components/RegisterForm'
import ToggleButtonNotEmpty from '../components/ToggleButton'
import { Button } from '@mui/material'

const Login = () => {
    // Local state for storing username and password
    const [username, setUsername] = useState('')
    const [password, setPassword] = useState('')
    const [loginSelected, setloginSelected] = useState<boolean>(true)
    const [errorMessage] = useState('')
    const [message, setMessage] = useState<{
        color: string
        message: string
    } | null>(null)

    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault()
        await login(username, password)
    }

    const handleRegister = async (e: React.FormEvent) => {
        e.preventDefault()
        await register(username, password)
    }

    async function login(username: string, password: string) {
        try {
            const response = await fetch(
                `http://api.localhost/users/${username}/login?password=${password}`,
                {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    credentials: 'include'
                }
            )

            if (!response.ok) {
                setMessage({ message: 'Invalid login', color: 'red' })
                return
            }

            const data = await response.json()
            const token = data.token
            const userId = data.userId
            sessionStorage.setItem('userId', userId)
            sessionStorage.setItem('token', token)
            setMessage({ message: 'Login successful', color: 'green' })
        } catch (error) {
            setMessage({
                message: `Error during logging in: ${error}`,
                color: 'red'
            })
        }
    }

    async function register(username: string, password: string) {
        const registerRequest = {
            name: username,
            password: password
        }

        try {
            const response = await fetch(`http://api.localhost/users/`, {
                method: 'POST',
                headers: {
                    'Access-Control-Allow-Origin': '*',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(registerRequest),
                credentials: 'include'
            })

            if (!response.ok) {
                setMessage({ message: 'Bad response', color: 'red' })
                return
            }

            const data = await response.json()
            const token = data.token
            const userId = data.userId
            sessionStorage.setItem('userId', userId)
            sessionStorage.setItem('token', token)
            setMessage({ message: 'Register successful', color: 'green' })
        } catch (error) {
            setMessage({
                message: `Error during register: ${error}`,
                color: 'red'
            })
        }
    }

    return (
        <div>
            {loginSelected ? (
                <div
                    style={{
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center'
                    }}
                >
                    <LoginForm
                        username={username}
                        password={password}
                        setUsername={setUsername}
                        setPassword={setPassword}
                        handleLogin={handleLogin}
                        errorMessage={errorMessage}
                        loginMessage={message}
                    />
                    <div
                        style={{
                            display: 'flex',
                            alignItems: 'center',
                            gap: '10px'
                        }}
                    >
                        <h4>Don't have an account?</h4>
                        <Button
                            style={{ marginTop: '5px' }}
                            onClick={() => setloginSelected(!loginSelected)}
                        >
                            Register
                        </Button>
                    </div>
                </div>
            ) : (
                <div
                    style={{
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center'
                    }}
                >
                    <RegisterForm
                        username={username}
                        password={password}
                        setUsername={setUsername}
                        setPassword={setPassword}
                        handleRegister={handleRegister}
                        errorMessage={errorMessage}
                        registerMessage={message}
                    />
                    <div
                        style={{
                            display: 'flex',
                            alignItems: 'center',
                            gap: '10px'
                        }}
                    >
                        <h4>Already have an account?</h4>
                        <Button
                            style={{ marginTop: '5px' }}
                            onClick={() => setloginSelected(!loginSelected)}
                        >
                            Login
                        </Button>
                    </div>
                </div>
            )}
        </div>
    )
}

export default Login
