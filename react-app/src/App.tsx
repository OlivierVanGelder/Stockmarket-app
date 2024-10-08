import { useState, useEffect } from "react";
import "./App.css";
import LineChart from "./components/LineChart";
import { BrowserRouter as Router, Route, Routes, Link } from "react-router-dom";
import Account from "./pages/Account";
import Graphs from "./pages/Graphs";
import Stock from "./pages/Stock";

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
            <Route path="/graphs" element={<Graphs />} />
            <Route path="/stock" element={<Stock />} />
            <Route path="/account" element={<Account />} />
          </Routes>
        </div>
      </Router>
    </div>
  );
}

export default App;
