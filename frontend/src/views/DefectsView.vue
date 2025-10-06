<template>
  <div class="defects-page">
    <div class="page-header">
      <div class="header-content">
        <h1>Дефекты</h1>
        <p class="header-subtitle">Отслеживание и управление дефектами</p>
      </div>
      <button class="add-button" @click="openAddModal" >
        <span class="button-icon">+</span>
        <span>Добавить дефект</span>
      </button>
    </div>
    
    <div class="filters-section">
      <div class="search-container">
        <input 
          v-model="searchQuery" 
          type="text" 
          placeholder="Поиск дефектов..." 
          class="search-input"
        />
        <span class="search-icon">🔍</span>
      </div>
      
      <select v-model="statusFilter" class="filter-select">
        <option value="">Все статусы</option>
        <option value="Открыт">Открыт</option>
        <option value="В работе">В работе</option>
        <option value="На проверке">На проверке</option>
        <option value="Закрыт">Закрыт</option>
      </select>
      
      <select v-model="priorityFilter" class="filter-select">
        <option value="">Все приоритеты</option>
        <option value="high">Высокий</option>
        <option value="medium">Средний</option>
        <option value="low">Низкий</option>
      </select>
    </div>
    
    <div class="table-container">
      <table class="defects-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Приоритет</th>
            <th>Описание</th>
            <th>Проект</th>
            <th>Назначено</th>
            <th>Статус</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          <tr 
            v-for="defect in filteredDefects" 
            :key="defect.id" 
            class="defect-row"
            @click="viewDefect(defect.id)"
          >
            <td class="defect-id">#{{ defect.id }}</td>
            <td>
              <div class="priority-badge" :class="defect.priority">
                {{ getPriorityText(defect.priority) }}
              </div>
            </td>
            <td class="defect-description">{{ defect.description }}</td>
            <td>{{ defect.project }}</td>
            <td>{{ defect.assignee }}</td>
            <td>
              <div class="status-badge" :class="getStatusClass(defect.status)">
                {{ defect.status }}
              </div>
            </td>
            <td>
              <div class="action-buttons">
                <button class="edit-button" title="Редактировать">
                  ✏️
                </button>
                <button class="delete-button" title="Удалить">
                  ×
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
      
      <div v-if="filteredDefects.length === 0" class="empty-state">
        <div class="empty-icon">🐛</div>
        <p>Дефекты не найдены</p>
      </div>
    </div>

    <AddDefectModal
      :is-open="showAddModal"
      :projects="projects"
      :users="users"
      @close="showAddModal = false"
      @submit="handleDefectSubmit"
    />
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import { useAuthStore } from '../store/auth';
import { useRouter } from 'vue-router';
import AddDefectModal from '../components/AddDefectModal.vue';

const auth = useAuthStore();
const router = useRouter();
const searchQuery = ref('');
const statusFilter = ref('');
const priorityFilter = ref('');
const showAddModal = ref(false);

const canCreate = computed(() => true);


const projects = ref([
  { id: 1, name: 'CRM Система' },
  { id: 2, name: 'Веб-портал' },
  { id: 3, name: 'Мобильное приложение' },
  { id: 4, name: 'Аналитическая система' }
]);

const users = ref([
  { id: 1, name: 'Иван Петров' },
  { id: 2, name: 'Мария Сидорова' },
  { id: 3, name: 'Алексей Иванов' },
  { id: 4, name: 'Дмитрий Смирнов' },
  { id: 5, name: 'Екатерина Волкова' }
]);

const defects = ref([
  {
    id: 1,
    description: 'Ошибка в форме логина',
    project: 'CRM Система',
    assignee: 'Иван',
    status: 'Открыт',
    priority: 'high'
  },
  {
    id: 2,
    description: 'Не грузится отчет',
    project: 'Веб-портал',
    assignee: 'Мария',
    status: 'В работе',
    priority: 'medium'
  },
  {
    id: 3,
    description: 'Проблема с авторизацией',
    project: 'Мобильное приложение',
    assignee: 'Алексей',
    status: 'На проверке',
    priority: 'low'
  },
  {
    id: 4,
    description: 'Медленная загрузка страницы',
    project: 'Веб-портал',
    assignee: 'Дмитрий',
    status: 'Открыт',
    priority: 'high'
  },
  {
    id: 5,
    description: 'Неправильное отображение графиков',
    project: 'Аналитическая система',
    assignee: 'Екатерина',
    status: 'Закрыт',
    priority: 'medium'
  }
]);

const filteredDefects = computed(() => {
  return defects.value.filter(defect => {
    const matchesSearch = !searchQuery.value || 
      defect.description.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      defect.project.toLowerCase().includes(searchQuery.value.toLowerCase());
    
    const matchesStatus = !statusFilter.value || defect.status === statusFilter.value;
    const matchesPriority = !priorityFilter.value || defect.priority === priorityFilter.value;
    
    return matchesSearch && matchesStatus && matchesPriority;
  });
});

const getStatusClass = (status) => {
  const statusClasses = {
    'Открыт': 'status-open',
    'В работе': 'status-progress',
    'На проверке': 'status-review',
    'Закрыт': 'status-closed'
  };
  return statusClasses[status] || 'status-default';
};

const getPriorityText = (priority) => {
  const priorityTexts = {
    'high': 'Высокий',
    'medium': 'Средний',
    'low': 'Низкий'
  };
  return priorityTexts[priority] || priority;
};

const openAddModal = () => {
  showAddModal.value = true;
};

const handleDefectSubmit = async (defectData) => {
  console.log('[v0] Submitting defect:', defectData);
  
  try {
    // TODO: Replace with actual API call
    // const response = await fetch('/api/defects', {
    //   method: 'POST',
    //   headers: { 'Content-Type': 'application/json' },
    //   body: JSON.stringify(defectData)
    // });
    
    // Mock: Add to local array
    const newDefect = {
      id: defects.value.length + 1,
      description: defectData.title,
      project: projects.value.find(p => p.id === defectData.projectId)?.name || 'Неизвестно',
      assignee: users.value.find(u => u.id === defectData.assigneeId)?.name || 'Не назначен',
      status: 'Открыт',
      priority: defectData.priority
    };
    
    defects.value.unshift(newDefect);
    
    alert('Дефект успешно создан!');
  } catch (error) {
    console.error('[v0] Error creating defect:', error);
    alert('Ошибка при создании дефекта');
  }
};

const viewDefect = (defectId) => {
  router.push(`/defects/${defectId}`);
};
</script>

<style scoped>
.defects-page {
  color: #2d3748;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 2rem;
  padding-bottom: 1.5rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
}

.header-content h1 {
  font-size: 2.25rem;
  font-weight: 700;
  color: white;
  margin-bottom: 0.5rem;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.header-subtitle {
  color: rgba(255, 255, 255, 0.9);
  font-size: 1rem;
  margin: 0;
}

.add-button {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: white;
  color: #667eea;
  padding: 0.75rem 1.25rem;
  border: none;
  border-radius: 12px;
  font-weight: 600;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.2s ease;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
}

.add-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 25px rgba(0, 0, 0, 0.2);
}

.button-icon {
  font-size: 1.25rem;
  line-height: 1;
}

.filters-section {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
}

.search-container {
  position: relative;
  flex: 1;
  max-width: 400px;
}

.search-input {
  width: 100%;
  padding: 0.75rem 1rem 0.75rem 2.5rem;
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  border: 2px solid transparent;
  border-radius: 12px;
  color: #2d3748;
  font-size: 0.95rem;
  outline: none;
  transition: all 0.2s ease;
}

.search-input:focus {
  border-color: #667eea;
  background: white;
  box-shadow: 0 0 0 4px rgba(102, 126, 234, 0.1);
}

.search-input::placeholder {
  color: #a0aec0;
}

.search-icon {
  position: absolute;
  left: 0.75rem;
  top: 50%;
  transform: translateY(-50%);
  font-size: 1rem;
}

.filter-select {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  border: 2px solid transparent;
  padding: 0.75rem 2.5rem 0.75rem 1rem;
  border-radius: 12px;
  color: #2d3748;
  font-size: 0.95rem;
  outline: none;
  transition: all 0.2s ease;
  appearance: none;
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' viewBox='0 0 24 24' fill='none' stroke='%234a5568' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Cpath d='M6 9l6 6 6-6'/%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 1rem center;
  background-size: 16px;
  cursor: pointer;
}

.filter-select:focus {
  border-color: #667eea;
  background-color: white;
  box-shadow: 0 0 0 4px rgba(102, 126, 234, 0.1);
}

.table-container {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
}

.defects-table {
  width: 100%;
  border-collapse: collapse;
}

.defects-table thead {
  background: #f7fafc;
}

.defects-table th {
  padding: 1rem;
  text-align: left;
  font-weight: 600;
  color: #4a5568;
  border-bottom: 2px solid #e2e8f0;
  font-size: 0.9rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.defects-table td {
  padding: 1rem;
  border-bottom: 1px solid #e2e8f0;
  color: #2d3748;
}

.defect-row:hover td {
  background-color: #f7fafc;
}

.defect-id {
  font-family: monospace;
  color: #718096;
  font-weight: 600;
}

.priority-badge {
  display: inline-block;
  padding: 0.35rem 0.75rem;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 600;
}

.priority-badge.high {
  background: #fed7d7;
  color: #c53030;
}

.priority-badge.medium {
  background: #feebc8;
  color: #c05621;
}

.priority-badge.low {
  background: #bee3f8;
  color: #2c5282;
}

.defect-description {
  max-width: 300px;
  font-weight: 500;
}

.status-badge {
  display: inline-block;
  padding: 0.35rem 0.75rem;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 600;
}

.status-open {
  background: #fed7d7;
  color: #c53030;
}

.status-progress {
  background: #bee3f8;
  color: #2c5282;
}

.status-review {
  background: #feebc8;
  color: #c05621;
}

.status-closed {
  background: #c6f6d5;
  color: #276749;
}

.action-buttons {
  display: flex;
  gap: 0.5rem;
}

.edit-button,
.delete-button {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s;
  padding: 0;
  border: none;
  font-size: 1.1rem;
}

.edit-button {
  background: #bee3f8;
  color: #2c5282;
}

.edit-button:hover {
  background: #90cdf4;
  transform: scale(1.1);
}

.delete-button {
  background: #fed7d7;
  color: #c53030;
  font-size: 1.5rem;
}

.delete-button:hover {
  background: #fc8181;
  transform: scale(1.1);
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  text-align: center;
}

.empty-icon {
  font-size: 4rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

.empty-state p {
  font-size: 1.25rem;
  color: #718096;
}

@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    gap: 1rem;
  }
  
  .filters-section {
    flex-direction: column;
  }
  
  .search-container {
    max-width: none;
  }
  
  .defects-table {
    min-width: 800px;
  }
  
  .table-container {
    overflow-x: auto;
  }
}
</style>
