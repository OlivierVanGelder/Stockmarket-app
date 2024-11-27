import { useState, useEffect } from 'react'
import './App.css'
import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom'
import Account from './pages/Account'
import Graphs from './pages/Graphs'
import Stock from './pages/Stock'
import ProtectedRoute from './components/ProtectedRoute'
import LoginForm from './components/LoginForm'

const Home = () => {
    // Local state for storing username and password
    const [username, setUsername] = useState('')
    const [password, setPassword] = useState('')
    const [errorMessage, setErrorMessage] = useState('')
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
            console.log(JSON.stringify(loginRequest))
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
                setLoginMessage({ message: 'Login failed', color: 'red' })
                throw new Error('Login failed')
            }

            const data = await response.json()
            const token = data.token
            const userId = data.userId
            console.log(data, userId, token)
            sessionStorage.setItem('userId', userId)
            sessionStorage.setItem('token', token)
            console.log('Login successful')
            setLoginMessage({ message: 'Login successful', color: 'green' })
        } catch (error) {
            setLoginMessage({
                message: 'Error during logging in',
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

function App() {
    return (
        <div className="App">
            <Router>
                <div>
                    <nav>
                        <ul>
                            <li>
                                <Link to="/graphs">Graphs</Link>
                            </li>
                            <li>
                                <Link to="/stock">Stock</Link>
                            </li>
                            <li>
                                <Link to="/account">Account</Link>
                            </li>
                        </ul>
                    </nav>

                    <Routes>
                        <Route path="/login" element={<Home />} />
                        <Route
                            path="/graphs"
                            element={
                                <ProtectedRoute>
                                    <Graphs />
                                </ProtectedRoute>
                            }
                        />
                        <Route
                            path="/stock"
                            element={
                                <ProtectedRoute>
                                    <Stock />
                                </ProtectedRoute>
                            }
                        />
                        <Route
                            path="/account"
                            element={
                                <ProtectedRoute>
                                    <Account />
                                </ProtectedRoute>
                            }
                        />
                    </Routes>
                </div>
            </Router>
        </div>
    )
}

export default App
