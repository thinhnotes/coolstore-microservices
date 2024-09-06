import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'

export default defineConfig(({ mode }) => {
  // Load env file based on `mode` in the current working directory.
  // Set the third parameter to '' to load all env regardless of the `VITE_` prefix.
  const env = loadEnv(mode, process.cwd(), '')

  return {
    plugins: [react()],
    resolve: {
      alias: {
        'components': path.resolve(__dirname, './src/components'),
        'pages': path.resolve(__dirname, './src/pages'),
        'services': path.resolve(__dirname, './src/services'),
        'stores': path.resolve(__dirname, './src/stores'),
      },
    },
    server: {
      port: env.PORT,
      proxy: {
        '^/_framework': {
          target: env.VITE_REACT_APP_BLAZOR,
          secure: false
        },
        '^/_content': {
          target: env.VITE_REACT_APP_BLAZOR,
          secure: false
        }
      },
    },
  }
})
