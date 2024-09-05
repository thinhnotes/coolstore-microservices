import React, { useEffect, useState, useCallback, memo } from "react";
import { Link } from "react-router-dom";
import {
  Container,
  Navbar,
  Nav,
  NavLink,
  UncontrolledDropdown,
  DropdownToggle,
  DropdownMenu,
  Collapse,
  Button,
} from "reactstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faShoppingCart,
  faUser,
  faSearch,
  faCubes,
} from "@fortawesome/free-solid-svg-icons";
import styled from "styled-components";
import { useNavigate } from "react-router-dom";

import { AuthService } from "services";
import { IAppUser } from "stores/types";

const Jumbotron = styled.div`
  padding: 2rem 1rem;
  margin-bottom: 2rem;
  background-color: #e9ecef;
  border-radius: 0.3rem;
`;

const StyledHeader = styled(Link)`
  text-decoration: none;
  &:hover {
    text-decoration: none;
    cursor: pointer;
  }
`;

const StyledNavBar = styled(Navbar)`
  margin-top: -3rem;
`;

const StyledDropdown = styled(UncontrolledDropdown)`
  display: inherit;
`;

const StyledNavLink = styled(NavLink)`
  &:hover {
    cursor: pointer;
  }
`;

interface IProps {}

const Header: React.FC<IProps> = () => {
  const [user, setUser] = useState<IAppUser | null>(null);
  const navigate = useNavigate();

  const fetchData = useCallback(async () => {
    var user = await AuthService.getUser();
    if (user && user.profile.name && user.profile.email) {
      let currentUser: IAppUser = {
        userName: user.profile.name,
        email: user.profile.email,
        accessToken: user.access_token,
      };
      setUser(currentUser);
    }
  }, []);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return (
    <>
      <Jumbotron>
        <Container fluid>
          <div>
            <h1 className="display-4">
              <StyledHeader to={`/`}>
                CoolStore Microservices on Dapr
              </StyledHeader>
            </h1>
            <p className="lead">
              A modern store uses technologies such as .NET Core, Docker,
              Kubernetes, Dapr, Tye etc.
            </p>
          </div>
        </Container>
      </Jumbotron>

      <div>
        <StyledNavBar color="light" light expand="md">
          {/* <Link className="navbar-brand" to="/">CoolStore</Link> */}
          <span className="lead">
            <FontAwesomeIcon icon={faSearch}></FontAwesomeIcon> Search&nbsp;
            <input type="textbox"></input>
          </span>
          <Collapse navbar>
            <Nav className="ml-auto">
              <StyledDropdown nav inNavbar>
                <StyledNavLink onClick={() => navigate(`/orders`)}>
                  <FontAwesomeIcon icon={faCubes}></FontAwesomeIcon> Orders
                </StyledNavLink>
                <StyledNavLink onClick={() => navigate(`/cart`)}>
                  <FontAwesomeIcon icon={faShoppingCart}></FontAwesomeIcon> Cart
                </StyledNavLink>
                <DropdownToggle nav caret>
                  <FontAwesomeIcon icon={faUser}></FontAwesomeIcon>{" "}
                  {user != null ? user.userName : ""}
                </DropdownToggle>
                <DropdownMenu>
                  <Button
                    color="link"
                    size="sm"
                    onClick={() => {
                      AuthService.signOut();
                    }}
                  >
                    Logout
                  </Button>
                </DropdownMenu>
              </StyledDropdown>
            </Nav>
          </Collapse>
        </StyledNavBar>
      </div>
    </>
  );
};

export default memo(Header);
