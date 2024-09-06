import { UserManager, UserManagerSettings } from 'oidc-client'
import LoggerService from './LoggerService'
import { useNavigate, useLocation } from 'react-router-dom'

const webUrl = window.location.origin
LoggerService.info(`Web URL is at ${webUrl}.`)

const OidcConfig: UserManagerSettings = {
  client_id: 'coolstore.web',
  redirect_uri: `${webUrl}/auth/callback`,
  authority: `${import.meta.env.VITE_REACT_APP_AUTHORITY}`,
    response_type: 'id_token token',
  post_logout_redirect_uri: `${webUrl}/`,
  scope: 'openid profile scope2',
  silent_redirect_uri: `${webUrl}/auth/silent-renew`,
  automaticSilentRenew: false,
  loadUserInfo: true
}

class AuthService {
  private userManager: UserManager

  constructor() {
    this.userManager = new UserManager(OidcConfig)
  }

  get UserManager(): UserManager {
    return this.userManager
  }

  async getUser() {
    return await this.userManager.getUser()
  }

  async authenticateUser(currentUrl: string) {
    if (!this.userManager || !this.userManager.getUser) {
      return
    }
    let oidcUser = await this.userManager.getUser()
    if (!oidcUser || oidcUser.expired) {
      LoggerService.debug('user is being authenticated...')
      let url = currentUrl
      await this.userManager.signinRedirect({ data: { url } })
    }
  }

  async signOut() {
    if (!this.userManager || !this.userManager.getUser) {
      return
    }

    let oidcUser = await this.userManager.getUser()
    if (oidcUser) {
      LoggerService.info('user is being logged out...')
      await this.userManager.signoutRedirect()
    }
  }
}

export default new AuthService()
