import { useEffect } from 'react'
import { useNavigate } from 'react-router-dom'

import { AuthService, LoggerService } from 'services'

export default function Callback() {
  const navigate = useNavigate();

  const signinRedirectCallback = async () => {
    console.log('Starting signinRedirectCallback');
    try {
      const user = await AuthService.UserManager.signinRedirectCallback()
      LoggerService.info('Successful token callback.')
      console.log('Navigating to user state URL:', user.state.url);
      navigate(user.state.url)
    } catch (error) {
      LoggerService.error(`There was an error while handling the token callback: ${error}.`)
      navigate('/401')
    }
  }

  useEffect(() => {
    signinRedirectCallback()
    // eslint-disable-next-line
  }, [])

  return <div>Authentication callback ...</div>
}
