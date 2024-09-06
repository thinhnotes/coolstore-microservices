import React, { useEffect } from 'react'
import { RouteProps } from 'react-router-dom'

import { Header, Footer, Notification, HeaderBlazor } from '../App'
import { AppActions, useStore } from '../../stores/store'

const withLayout = (WrappedComponent: React.ComponentType) => {
  return function ({ ...props }) {
    const { dispatch } = useStore()

    useEffect(() => {
      const timer = setTimeout(() => {
        dispatch(AppActions.hideNotification())
      }, 3000)
      return () => clearTimeout(timer)
    })

    return (
      <>
        <Header></Header>
        <HeaderBlazor></HeaderBlazor>
        <Notification></Notification>
        <WrappedComponent {...(props)} />
        <Footer></Footer>
      </>
    )
  }
}

export default withLayout
