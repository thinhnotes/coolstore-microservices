import React from 'react'
import { createRoot } from 'react-dom/client'
import { ThemeProvider } from 'styled-components'

import 'bootstrap/dist/css/bootstrap.min.css'
import { App } from './components/App'
import { AppProvider } from 'stores'
import * as serviceWorker from './serviceWorker'

const theme = {}

const container = document.getElementById('root')
const root = createRoot(container!)

function Root() {

  return (
    <React.StrictMode>
      <ThemeProvider theme={theme}>
        <AppProvider>
          <App />
        </AppProvider>
      </ThemeProvider>
    </React.StrictMode>
  );
}

root.render(<Root />)

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister()
