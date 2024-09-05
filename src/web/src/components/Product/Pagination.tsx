import { useEffect, useCallback, useState } from 'react'
import { AppActions, useStore } from 'stores/store'
import styled from 'styled-components'
import { searchProducts } from 'services/ProductService'

const StyledNav = styled.nav`
  margin-top: 25px;
`
interface ICategoryTagModel {
  key: string;
  count: number;
}

interface IInventoryTagModel {
  key: string;
  count: number;
}

const Pagination: React.FC = () => {
  const { state, dispatch } = useStore()
  const [categoryTags, setCategoryTags] = useState<ICategoryTagModel[]>([])
  const [inventoryTags, setInventoryTags] = useState<IInventoryTagModel[]>([])
  const pageSize = 9;
  const currentPage = 1;
  const fetchData = useCallback(
    async (page: number, price: number) => {
      const result = await searchProducts('*', price, page)
      setCategoryTags(result.categoryTags)
      setInventoryTags(result.inventoryTags)
      dispatch(AppActions.loadProducts(result.products))
    },
    [dispatch]
  )
  useEffect(() => {
    fetchData(1, 10000)
  }, [state.isProductsLoaded, fetchData])


  return (
    <>
      <StyledNav aria-label="Page navigation example">
        <div>Total items: {state.products.length}</div>
        <div>Total pages : {Math.ceil(state.products.length / pageSize)}</div>
        <ul className="pagination">
          <li className="page-item">
            <a className="page-link" href="{someValidPath}">
              Previous
            </a>
          </li>
          {
            Array.from({ length: Math.ceil(state.products.length / pageSize) }).map((_, index) => (
              <li className={`page-item ${index + 1 === currentPage ? 'active' : ''}`} key={index}>
                <a className="page-link" href="{someValidPath}">
                  {index + 1}
                </a>
              </li>
            ))
          }          
          <li className="page-item">
            <a className="page-link" href="{someValidPath}">
              Next
            </a>
          </li>
        </ul>
      </StyledNav>
    </>
  )
}

export default Pagination
