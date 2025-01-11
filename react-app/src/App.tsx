import './App.css'
import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom'
import Account from './pages/Account'
import Graphs from './pages/Graphs'
import Stock from './pages/Stock'
import ProtectedRoute from './components/ProtectedRoute'
import Login from './pages/Login'
import Home from './pages/Home'
import React from 'react'

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
                        <Route
                            path="/"
                            element={
                                <ProtectedRoute>
                                    <Home />
                                </ProtectedRoute>
                            }
                        />
                        <Route path="/login" element={<Login />} />
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
