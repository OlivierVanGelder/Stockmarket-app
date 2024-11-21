import { useState, useEffect } from 'react'
import './App.css'
import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom'
import Account from './pages/Account'
import Graphs from './pages/Graphs'
import Stock from './pages/Stock'
import ProtectedRoute from './components/ProtectedRoute'

const Home = () => {
    // Local state for storing username and password
    const [username, setUsername] = useState('')
    const [password, setPassword] = useState('')
    const [errorMessage, setErrorMessage] = useState('')

    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault()
        await login(username, password)
    }

    return (
        <div>
            <h1>Login to StockEngine</h1>
            {errorMessage && <p style={{ color: 'red' }}>{errorMessage}</p>}
            <form onSubmit={handleLogin}>
                <div>
                    <label htmlFor="username">Username</label>
                    <input
                        type="text"
                        id="username"
                        value={username}
                        onChange={e => setUsername(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label htmlFor="password">Password</label>
                    <input
                        type="password"
                        id="password"
                        value={password}
                        onChange={e => setPassword(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <button type="submit">Login</button>
                </div>
            </form>
        </div>
    )
}

async function login(username: string, password: string) {
    const loginRequest = {
        name: username,
        password: password
    }

    try {
        console.log(JSON.stringify(loginRequest))
        const response = await fetch('https://localhost:42069/accounts/login', {
            method: 'POST',
            headers: {
                'Access-Control-Allow-Origin': '*',
                'Content-Type': 'application/json'
            },
            credentials: 'include',
            body: JSON.stringify(loginRequest)
        })

        if (!response.ok) {
            throw new Error('Login failed')
        }

        const data = await response.json()
        const token = data.Token

        // Store token in localStorage or use it as needed
        localStorage.setItem('token', token)
        console.log('Login successful, token:', token)
    } catch (error) {
        console.error('Error during login:', error)
    }
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
