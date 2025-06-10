// components/Layout.jsx
import { AppBar, Toolbar, Typography, Button, Container } from '@mui/material';
import { Link } from 'react-router-dom';
import LogoutButton from './LogoutButton';

const Layout = ({ children }) => {
  const isAuthenticated = !!localStorage.getItem('token');
  
  return (
    <>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            <Link to="/" style={{ color: 'inherit', textDecoration: 'none' }}>
              Hotel Paradise
            </Link>
          </Typography>
          <Button color="inherit" component={Link} to="/rooms">
            Rooms
          </Button>
          {isAuthenticated ? (
            <>
              <Button color="inherit" component={Link} to="/profile">
                My Bookings
              </Button>
              <LogoutButton />
            </>
          ) : (
            <Button color="inherit" component={Link} to="/login">
              Login
            </Button>
          )}
        </Toolbar>
      </AppBar>
      <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
        {children}
      </Container>
    </>
  );
};

export default Layout;