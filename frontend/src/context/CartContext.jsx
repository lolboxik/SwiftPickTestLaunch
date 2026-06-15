import { createContext, useState, useContext, useEffect, useCallback } from 'react';
import { cartService } from '../api/services';
import { useAuth } from './AuthContext';

const CartContext = createContext(null);

export function CartProvider({ children }) {
  const { isAuthenticated } = useAuth();
  const [cart, setCart] = useState(null);
  const [loading, setLoading] = useState(false);

  const refreshCart = useCallback(async () => {
    if (!isAuthenticated) {
      setCart(null);
      return;
    }

    try {
      setLoading(true);
      const response = await cartService.get();
      setCart(response.data);
    } catch (error) {
      if (error.response?.status !== 401) {
        console.error('Ошибка загрузки корзины:', error);
      }
      setCart(null);
    } finally {
      setLoading(false);
    }
  }, [isAuthenticated]);

  useEffect(() => {
    refreshCart();
  }, [refreshCart]);

  const addToCart = async (productId, quantity = 1) => {
    try {
      const response = await cartService.add(productId, quantity);
      setCart(response.data);
      return response.data;
    } catch (error) {
      console.error('Ошибка добавления в корзину:', error);
      throw error;
    }
  };

  const updateCartItem = async (productId, quantity) => {
    try {
      const response = await cartService.update(productId, quantity);
      setCart(response.data);
      return response.data;
    } catch (error) {
      console.error('Ошибка обновления корзины:', error);
      throw error;
    }
  };

  const removeFromCart = async (productId) => {
    try {
      const response = await cartService.remove(productId);
      setCart(response.data);
      return response.data;
    } catch (error) {
      console.error('Ошибка удаления из корзины:', error);
      throw error;
    }
  };

  const clearCart = async () => {
    try {
      const response = await cartService.clear();
      setCart(response.data);
      return response.data;
    } catch (error) {
      console.error('Ошибка очистки корзины:', error);
      throw error;
    }
  };

  const value = {
    cart,
    loading,
    refreshCart,
    addToCart,
    updateCartItem,
    removeFromCart,
    clearCart,
    itemCount: cart?.items?.reduce((sum, item) => sum + item.quantity, 0) || 0,
    totalAmount: cart?.totalAmount || 0
  };

  return <CartContext.Provider value={value}>{children}</CartContext.Provider>;
}

export function useCart() {
  const context = useContext(CartContext);
  if (!context) {
    throw new Error('useCart должен использоваться внутри CartProvider');
  }
  return context;
}
