import { createBrowserRouter } from "react-router";
import { Layout } from "./components/Layout";
import { Onboarding } from "./components/Onboarding";
import { Home } from "./components/Home";
import { RecipeDetail } from "./components/RecipeDetail";
import { StoreLocator } from "./components/StoreLocator";
import { UserProfile } from "./components/UserProfile";
import { Despensa } from "./components/Despensa";

export const router = createBrowserRouter([
  {
    path: "/",
    Component: Layout,
    children: [
      { index: true, Component: Onboarding },
      { path: "home", Component: Home },
      { path: "receta/:id", Component: RecipeDetail },
      { path: "tiendas", Component: StoreLocator },
      { path: "despensa", Component: Despensa },
      { path: "perfil", Component: UserProfile },
    ],
  },
]);
