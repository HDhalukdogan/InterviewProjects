import React from 'react';
import { Outlet } from 'react-router';
import Navbar from './Navbar';

function Layout() {
  return (
    <>
      <Navbar />
      <main style={{ minHeight: 'calc(100vh - 72px)', boxSizing: 'border-box' }}>
        <Outlet />
      </main>
    </>
  );
}

export default Layout;
