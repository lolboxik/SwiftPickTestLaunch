import api from './api';

export const authService = {
  async login(email, password) {
    const response = await api.post('/auth/login', { email, password });
    if (response.data.success) {
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('user', JSON.stringify({
        id: response.data.userId,
        email: response.data.email,
        firstName: response.data.firstName,
        lastName: response.data.lastName,
        roles: response.data.roles
      }));
    }
    return response.data;
  },

  async register(email, password, firstName, lastName) {
    const response = await api.post('/auth/register', {
      email,
      password,
      firstName,
      lastName
    });
    if (response.data.success) {
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('user', JSON.stringify({
        id: response.data.userId,
        email: response.data.email,
        firstName: response.data.firstName,
        lastName: response.data.lastName,
        roles: response.data.roles
      }));
    }
    return response.data;
  },

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  },

  getCurrentUser() {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  },

  getToken() {
    return localStorage.getItem('token');
  },

  async changePassword(currentPassword, newPassword) {
    const response = await api.post('/auth/change-password', {
      currentPassword,
      newPassword
    });
    return response.data;
  },

  isAuthenticated() {
    return !!localStorage.getItem('token');
  },

  isAdmin() {
    const user = this.getCurrentUser();
    return user?.roles?.includes('Admin');
  }
};

export const productService = {
  getAll() {
    return api.get('/products');
  },

  getById(id) {
    return api.get(`/products/${id}`);
  },

  getByCategory(categoryId) {
    return api.get(`/products/category/${categoryId}`);
  },

  search(query) {
    return api.get('/products/search', { params: { query } });
  },

  filter(params) {
    return api.get('/products/filter', { params });
  },

  create(product) {
    return api.post('/products', product);
  },

  update(id, product) {
    return api.put(`/products/${id}`, product);
  },

  delete(id) {
    return api.delete(`/products/${id}`);
  }
};

export const categoryService = {
  getAll() {
    return api.get('/categories');
  },

  getById(id) {
    return api.get(`/categories/${id}`);
  },

  create(category) {
    return api.post('/categories', category);
  },

  update(id, category) {
    return api.put(`/categories/${id}`, category);
  },

  delete(id) {
    return api.delete(`/categories/${id}`);
  }
};

export const cartService = {
  get() {
    return api.get('/cart');
  },

  add(productId, quantity) {
    return api.post('/cart/add', { productId, quantity });
  },

  update(productId, quantity) {
    return api.put('/cart/update', { productId, quantity });
  },

  remove(productId) {
    return api.delete(`/cart/remove/${productId}`);
  },

  clear() {
    return api.delete('/cart/clear');
  }
};

export const orderService = {
  getAll() {
    return api.get('/orders');
  },

  getById(id) {
    return api.get(`/orders/${id}`);
  },

  create(order) {
    return api.post('/orders', order);
  },

  getAllAdmin() {
    return api.get('/orders/admin/all');
  },

  updateStatus(id, status) {
    return api.put(`/orders/admin/${id}/status`, { status });
  }
};

export const adminService = {
  getUsers() {
    return api.get('/admin/users');
  },

  updateUserRole(id, role) {
    return api.put(`/admin/users/${id}/role`, { role });
  },

  activateUser(id, isActive) {
    return api.put(`/admin/users/${id}/activate`, { isActive });
  },

  getRoles() {
    return api.get('/admin/roles');
  },

  getDashboard() {
    return api.get('/admin/dashboard');
  }
};
