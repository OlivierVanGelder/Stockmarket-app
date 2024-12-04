import React from 'react'
import { useState } from 'react'
import LoginForm from '../components/LoginForm'

const Login = () => {
    // Local state for storing username and password
    const [username, setUsername] = useState('')
    const [password, setPassword] = useState('')
    const [errorMessage] = useState('')
    const [loginMessage, setLoginMessage] = useState<{
        color: string
        message: string
    } | null>(null)

    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault()
        await login(username, password)
    }

    async function login(username: string, password: string) {
        const loginRequest = {
            name: username,
            password: password
        }

        try {
            const response = await fetch(
                'https://localhost:42069/accounts/login/verify',
                {
                    method: 'POST',
                    headers: {
                        'Access-Control-Allow-Origin': '*',
                        'Content-Type': 'application/json'
                    },
                    credentials: 'include',
                    body: JSON.stringify(loginRequest)
                }
            )

            if (!response.ok) {
                setLoginMessage({ message: 'Invalid login', color: 'red' })
                return
            }

            const data = await response.json()
            const token = data.token
            const userId = data.userId
            sessionStorage.setItem('userId', userId)
            sessionStorage.setItem('token', token)
            setLoginMessage({ message: 'Login successful', color: 'green' })
        } catch (error) {
            setLoginMessage({
                message: `Error during logging in: ${error}`,
                color: 'red'
            })
        }
    }

    return (
        <div>
            <LoginForm
                username={username}
                password={password}
                setUsername={setUsername}
                setPassword={setPassword}
                handleLogin={handleLogin}
                errorMessage={errorMessage}
                loginMessage={loginMessage}
            />
        </div>
    )
}

export default Login
