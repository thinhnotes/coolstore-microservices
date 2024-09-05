import React, { Suspense, lazy, useCallback } from 'react'
import { BrowserRouter, Routes, Route, useNavigate } from 'react-router-dom'
import { startTransition } from 'react'

import { withAuth } from 'components/HOC'
import { Callback, SilentCallback, NotAuth } from 'pages/Authentication'
import NotFound from 'pages/404'

const Home = lazy(() => import('pages/Home'))
const ProductDetail = lazy(() => import('pages/ProductDetail'))
const Cart = lazy(() => import('pages/Cart'))
const Orders = lazy(() => import('pages/Order'))

const AppLayout = () => {
  return (
    <BrowserRouter>
      <Suspense fallback={<div>Loading...</div>}>
        <Routes>
          <Route path={'/'} Component={withAuth(Home)} />
          <Route path={'/product/:id'} Component={withAuth(ProductDetail)} />
          <Route path={'/cart'} Component={withAuth(Cart)} />
          <Route path={'/orders'} Component={withAuth(Orders)} />
          <Route path={'/auth/callback'} element={<Callback />} />
          <Route path={'/auth/silent-renew'} element={<SilentCallback />} />
          <Route path={'/401'} element={<NotAuth />} />
          <Route path={'*'} element={<NotFound />} />
        </Routes>
      </Suspense>
    </BrowserRouter>
  )
}

export default AppLayout
