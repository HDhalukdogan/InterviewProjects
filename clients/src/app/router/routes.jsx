import { createBrowserRouter } from "react-router";
import Appointment from "../../features/appointment/Appointment";
import App from "../../App";
import Cart from "../../features/cart/Cart";


export const router = createBrowserRouter([
  {
    path: "/",
    element: <App />
  },
  {
    path: "/appointment",
    element: <Appointment />
  },
  {
    path: "/cart",
    element: <Cart />
  }
])