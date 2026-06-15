import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { adminService, productService, categoryService, orderService } from '../api/services';
import './Admin.css';

export default function Admin() {
  const { isAdmin } = useAuth();
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState('dashboard');
  const [users, setUsers] = useState([]);
  const [products, setProducts] = useState([]);
  const [orders, setOrders] = useState([]);
  const [stats, setStats] = useState(null);
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showProductForm, setShowProductForm] = useState(false);
  const [editingProduct, setEditingProduct] = useState(null);
  const [productForm, setProductForm] = useState({
    name: '',
    description: '',
    price: '',
    stock: '',
    categoryId: '',
    brand: '',
    imagePath: ''
  });

  useEffect(() => {
    loadData();
  }, []);

  async function loadData() {
    setLoading(true);
    try {
      const [statsRes, usersRes, productsRes, ordersRes, categoriesRes] = await Promise.all([
        adminService.getDashboard(),
        adminService.getUsers(),
        productService.getAll(),
        orderService.getAllAdmin(),
        categoryService.getAll()
      ]);
      setStats(statsRes.data);
      setUsers(usersRes.data);
      setProducts(productsRes.data);
      setOrders(ordersRes.data);
      setCategories(categoriesRes.data);
    } catch (error) {
      console.error('Ошибка загрузки:', error);
    } finally {
      setLoading(false);
    }
  }

  const handleUpdateRole = async (userId, role) => {
    try {
      await adminService.updateUserRole(userId, role);
      loadData();
    } catch (error) {
      alert(error.response?.data?.message || 'Ошибка обновления роли');
    }
  };

  const handleDeleteProduct = async (id) => {
    if (confirm('Удалить товар?')) {
      await productService.delete(id);
      loadData();
    }
  };

  const handleEditProduct = (product) => {
    setEditingProduct(product);
    setProductForm({
      name: product.name,
      description: product.description || '',
      price: product.price.toString(),
      stock: product.stock.toString(),
      categoryId: product.categoryId.toString(),
      brand: product.brand || '',
      imagePath: product.imagePath || ''
    });
    setShowProductForm(true);
  };

  const handleCreateProduct = () => {
    setEditingProduct(null);
    setProductForm({
      name: '',
      description: '',
      price: '',
      stock: '',
      categoryId: '',
      brand: '',
      imagePath: ''
    });
    setShowProductForm(true);
  };

  const handleSaveProduct = async () => {
    try {
      const productData = {
        ...productForm,
        price: parseFloat(productForm.price),
        stock: parseInt(productForm.stock),
        categoryId: parseInt(productForm.categoryId)
      };

      if (editingProduct) {
        await productService.update(editingProduct.id, productData);
      } else {
        await productService.create(productData);
      }
      setShowProductForm(false);
      loadData();
    } catch (error) {
      alert('Ошибка сохранения товара');
    }
  };

  const handleUpdateOrderStatus = async (orderId, status) => {
    try {
      await orderService.updateStatus(orderId, status);
      loadData();
    } catch (error) {
      alert(error.response?.data?.message || 'Ошибка обновления статуса');
    }
  };

  if (!isAdmin) {
    navigate('/');
    return null;
  }

  if (loading) return <div className="loading">Загрузка...</div>;

  return (
    <div className="admin-page">
      <h1>Админ-панель</h1>

      <div className="admin-tabs">
        <button className={activeTab === 'dashboard' ? 'active' : ''} onClick={() => setActiveTab('dashboard')}>📊 Дашборд</button>
        <button className={activeTab === 'users' ? 'active' : ''} onClick={() => setActiveTab('users')}>👥 Пользователи</button>
        <button className={activeTab === 'products' ? 'active' : ''} onClick={() => setActiveTab('products')}>📦 Товары</button>
        <button className={activeTab === 'orders' ? 'active' : ''} onClick={() => setActiveTab('orders')}>🛒 Заказы</button>
      </div>

      <div className="admin-content">
        {activeTab === 'dashboard' && (
          <div className="dashboard-stats">
            <div className="stat-card">
              <div className="stat-value">{stats?.totalUsers || 0}</div>
              <div className="stat-label">Пользователей</div>
            </div>
            <div className="stat-card">
              <div className="stat-value">{products.length}</div>
              <div className="stat-label">Товаров</div>
            </div>
            <div className="stat-card">
              <div className="stat-value">{orders.length}</div>
              <div className="stat-label">Заказов</div>
            </div>
            <div className="stat-card">
              <div className="stat-value">{(stats?.revenue || 0).toLocaleString()} ₽</div>
              <div className="stat-label">Выручка</div>
            </div>
          </div>
        )}

        {activeTab === 'users' && (
          <table className="admin-table">
            <thead>
              <tr>
                <th>Email</th>
                <th>Имя</th>
                <th>Роль</th>
                <th>Статус</th>
                <th>Действия</th>
              </tr>
            </thead>
            <tbody>
              {users.map(user => (
                <tr key={user.id}>
                  <td>{user.email}</td>
                  <td>{user.firstName || '-'} {user.lastName || ''}</td>
                  <td>
                    <select value={user.roles?.[0] || 'User'} onChange={(e) => handleUpdateRole(user.id, e.target.value)}>
                      <option value="User">User</option>
                      <option value="Admin">Admin</option>
                      <option value="Manager">Manager</option>
                    </select>
                  </td>
                  <td>{user.isActive ? '✅' : '❌'}</td>
                  <td>
                    <button onClick={() => adminService.activateUser(user.id, !user.isActive).then(loadData).catch(() => alert('Ошибка изменения статуса'))}>
                      {user.isActive ? 'Заблокировать' : 'Разблокировать'}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}

        {activeTab === 'products' && (
          <div>
            <button className="create-btn" onClick={handleCreateProduct}>+ Добавить товар</button>
            <table className="admin-table">
              <thead>
                <tr>
                  <th>Название</th>
                  <th>Цена</th>
                  <th>Остаток</th>
                  <th>Категория</th>
                  <th>Действия</th>
                </tr>
              </thead>
              <tbody>
                {products.map(product => (
                  <tr key={product.id}>
                    <td>{product.name}</td>
                    <td>{product.price.toLocaleString()} ₽</td>
                    <td>{product.stock}</td>
                    <td>{product.categoryName}</td>
                    <td>
                      <button className="edit-btn" onClick={() => handleEditProduct(product)}>Изменить</button>
                      <button className="delete-btn" onClick={() => handleDeleteProduct(product.id)}>Удалить</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {activeTab === 'orders' && (
          <table className="admin-table">
            <thead>
              <tr>
                <th>№ заказа</th>
                <th>Клиент</th>
                <th>Сумма</th>
                <th>Статус</th>
                <th>Действия</th>
              </tr>
            </thead>
            <tbody>
              {orders.map(order => (
                <tr key={order.id}>
                  <td>{order.orderNumber}</td>
                  <td>{order.userEmail}</td>
                  <td>{order.totalAmount.toLocaleString()} ₽</td>
                  <td>
                    <select value={order.status} onChange={(e) => handleUpdateOrderStatus(order.id, e.target.value)}>
                      <option value="New">Новый</option>
                      <option value="Paid">Оплачен</option>
                      <option value="Shipped">Отправлен</option>
                      <option value="Delivered">Доставлен</option>
                      <option value="Cancelled">Отменён</option>
                    </select>
                  </td>
                  <td>-</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {showProductForm && (
        <div className="modal-overlay" onClick={() => setShowProductForm(false)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <h2>{editingProduct ? 'Редактировать товар' : 'Новый товар'}</h2>
            <div className="form-group">
              <label>Название</label>
              <input value={productForm.name} onChange={(e) => setProductForm({...productForm, name: e.target.value})} />
            </div>
            <div className="form-group">
              <label>Описание</label>
              <textarea value={productForm.description} onChange={(e) => setProductForm({...productForm, description: e.target.value})} />
            </div>
            <div className="form-row">
              <div className="form-group">
                <label>Цена (₽)</label>
                <input type="number" value={productForm.price} onChange={(e) => setProductForm({...productForm, price: e.target.value})} />
              </div>
              <div className="form-group">
                <label>Остаток (шт.)</label>
                <input type="number" value={productForm.stock} onChange={(e) => setProductForm({...productForm, stock: e.target.value})} />
              </div>
            </div>
            <div className="form-row">
              <div className="form-group">
                <label>Категория</label>
                <select value={productForm.categoryId} onChange={(e) => setProductForm({...productForm, categoryId: e.target.value})}>
                  <option value="">Выберите категорию</option>
                  {categories.map(cat => (
                    <option key={cat.id} value={cat.id}>{cat.name}</option>
                  ))}
                </select>
              </div>
              <div className="form-group">
                <label>Бренд</label>
                <input value={productForm.brand} onChange={(e) => setProductForm({...productForm, brand: e.target.value})} />
              </div>
            </div>
            <div className="form-group">
              <label>Путь к изображению</label>
              <input value={productForm.imagePath} onChange={(e) => setProductForm({...productForm, imagePath: e.target.value})} />
            </div>
            <div className="modal-actions">
              <button className="cancel-btn" onClick={() => setShowProductForm(false)}>Отмена</button>
              <button className="save-btn" onClick={handleSaveProduct}>Сохранить</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
