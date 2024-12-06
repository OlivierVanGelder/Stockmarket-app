import React, { ChangeEvent, FormEvent } from 'react'

// Define types for props
interface RegisterFormProps {
    username: string
    password: string
    setUsername: (username: string) => void
    setPassword: (password: string) => void
    handleRegister: (event: FormEvent<HTMLFormElement>) => void
    errorMessage: string
    registerMessage: { color: string; message: string } | null
}

// Reusable Input Component
const InputField: React.FC<{
    label: string
    id: string
    type: string
    value: string
    onChange: (e: ChangeEvent<HTMLInputElement>) => void
    required: boolean
}> = ({ label, id, type, value, onChange, required }) => (
    <div style={{ marginBottom: '1rem' }}>
        <label htmlFor={id} style={{ display: 'block', fontWeight: 'bold' }}>
            {label}
        </label>
        <input
            type={type}
            id={id}
            value={value}
            onChange={onChange}
            required={required}
            style={{
                width: '100%',
                padding: '0.5rem',
                borderRadius: '5px',
                border: '1px solid #ccc',
                marginTop: '0.5rem'
            }}
        />
    </div>
)

// Reusable Button Component
const Button: React.FC<{
    text: string
    type?: 'button' | 'submit' | 'reset'
    onClick?: () => void
}> = ({ text, type = 'button', onClick }) => (
    <button
        type={type}
        onClick={onClick}
        style={{
            backgroundColor: '#007BFF',
            color: 'white',
            padding: '0.75rem 1.5rem',
            border: 'none',
            borderRadius: '5px',
            cursor: 'pointer',
            fontSize: '1rem'
        }}
    >
        {text}
    </button>
)

// Login Form Component
const registerForm: React.FC<RegisterFormProps> = ({
    username,
    password,
    setUsername,
    setPassword,
    handleRegister,
    errorMessage,
    registerMessage: registerMessage
}) => {
    return (
        <div
            style={{
                maxWidth: '400px',
                margin: '2rem auto',
                padding: '2rem',
                border: '1px solid #ddd',
                borderRadius: '10px',
                boxShadow: '0px 4px 10px rgba(0, 0, 0, 0.1)',
                backgroundColor: '#fff'
            }}
        >
            <h1 style={{ textAlign: 'center', marginBottom: '2rem' }}>
                Register to StockEngine
            </h1>
            {errorMessage && (
                <p style={{ color: 'red', textAlign: 'center' }}>
                    {errorMessage}
                </p>
            )}
            <form onSubmit={handleRegister}>
                <InputField
                    label="Username"
                    id="username"
                    type="text"
                    value={username}
                    onChange={e => setUsername(e.target.value)}
                    required
                />
                <InputField
                    label="Password"
                    id="password"
                    type="password"
                    value={password}
                    onChange={e => setPassword(e.target.value)}
                    required
                />
                <div style={{ textAlign: 'center', marginTop: '1.5rem' }}>
                    <Button text="Register" type="submit" />
                </div>
                {registerMessage && (
                    <p
                        style={{
                            color: registerMessage.color,
                            textAlign: 'center',
                            marginTop: '1rem'
                        }}
                    >
                        {registerMessage.message}
                    </p>
                )}
            </form>
        </div>
    )
}

export default registerForm
