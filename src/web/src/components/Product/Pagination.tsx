import React from 'react'
import styled from 'styled-components'

const StyledNav = styled.nav`
  margin-top: 25px;
`

const Pagination: React.FC = () => {
  return (
    <>
      <StyledNav aria-label="Page navigation example">
        <ul className="pagination">
          <li className="page-item">
            <a className="page-link" href="{someValidPath}">
              Previous
            </a>
          </li>
          <li className="page-item">
            <a className="page-link" href="{someValidPath}">
              1
            </a>
          </li>
          <li className="page-item active">
            <a className="page-link " href="{someValidPath}">
              2
            </a>
          </li>
          <li className="page-item">
            <a className="page-link" href="{someValidPath}">
              3
            </a>
          </li>
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
