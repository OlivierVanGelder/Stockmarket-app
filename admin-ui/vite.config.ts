import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
    base: '/',
    publicDir: 'public',
    plugins: [react()],
    server: {
        port: 3010, // Set the desired port here
        strictPort: true // Ensures the app fails to start if the port is already in use
    }
})
