import React, { useEffect, useCallback } from 'react'
import { useParams } from 'react-router-dom'

import { ProductItemDetail } from 'components/Product'
import { withLayout } from 'components/HOC'

import { useStore, AppActions } from 'stores/store'
import { getProduct } from 'services/ProductService'

const ProductDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const { state, dispatch } = useStore()

  const fetchData = useCallback(
    async (id: string) => {
      const product = await getProduct(id)
      dispatch(AppActions.loadProduct(product))
    },
    [dispatch]
  )

  useEffect(() => {
    if (id) {
      fetchData(id)
    }
  }, [state.isProductLoaded, fetchData, id])

  return (
    <div className="container-fluid">
      <div className="row">
        <div className="offset-xl-2 col-xl-8 col-lg-12 col-md-12 col-sm-12 col-12">
          <div className="row">
            {state.isProductLoaded && state.productDetail && <ProductItemDetail data={state.productDetail} />}
          </div>
        </div>
      </div>
    </div>
  )
}

export default withLayout(ProductDetail)
