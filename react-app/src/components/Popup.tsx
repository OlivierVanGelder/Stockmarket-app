import React from 'react'
import { useState } from 'react'
import './Popup.css'

interface PopupProps {
    isBuy: boolean
    isOpen: boolean
    onClose: () => void
    onSubmit: (amount: number) => void
    stockPrice: number
    userBalance: number
}

const Popup: React.FC<PopupProps> = ({
    isBuy,
    isOpen,
    onClose,
    onSubmit,
    stockPrice,
    userBalance
}) => {
    const [amount, setAmount] = useState(0)

    const handleAmountChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setAmount(Number(e.target.value))
    }

    const handleSubmit = () => {
        if (isBuy) {
            if (amount > 0 && amount <= userBalance / stockPrice) {
                onSubmit(amount)
            } else {
                alert('Insufficient funds or invalid amount')
            }
        } else {
            if (amount > 0) {
                onSubmit(amount)
            } else {
                alert('Invalid amount')
            }
        }
    }

    return isOpen ? (
        <div className="popup-overlay">
            <div className="popup-content">
                {isBuy ? <h2>Invest in Stock</h2> : <h2>Sell Stock</h2>}
                <p>Price per stock: ${stockPrice}</p>
                <input
                    type="number"
                    min="1"
                    value={amount}
                    onChange={handleAmountChange}
                    placeholder="Amount"
                />
                <div>
                    {isBuy ? (
                        <button onClick={handleSubmit}>Confirm Purchase</button>
                    ) : (
                        <button onClick={handleSubmit}>Confirm Sale</button>
                    )}
                    <button onClick={onClose}>Cancel</button>
                </div>
            </div>
        </div>
    ) : null
}

export default Popup
