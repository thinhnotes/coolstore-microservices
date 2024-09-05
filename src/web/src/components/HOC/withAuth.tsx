import React, { useEffect, useCallback } from 'react'
import { useLocation, Location, RouteProps } from 'react-router-dom'
import { AuthService } from 'services'

const withAuth = <P extends object>(WrappedComponent: React.ComponentType<P>) => {
  
  return function({ ...props }: P & RouteProps) {
    const authUser = useCallback(async () => {
      await AuthService.authenticateUser('/')
    }, [props])

    useEffect(() => {
      authUser()
    }, [authUser])

    return <WrappedComponent {...props} />
  }
}

export default withAuth
