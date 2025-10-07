<template>
  <div class="defect-detail-page">
    <div class="page-header">
      <button class="back-button" @click="goBack">
        <span>←</span>
        <span>Назад к списку</span>
      </button>
      
      <div class="header-actions">
        <button 
          v-if="!isEditing"
          class="action-button edit"
          @click="isEditing = true"
        >
          <span>✏️</span>
          <span>Редактировать</span>
        </button>

        <button 
          class="action-button save"
          v-else
          @click="saveChanges"
        >
          <span>💾</span>
          <span>Сохранить</span>
        </button>

        <button 
          class="action-button delete"
          @click="handleDelete"
        >
          <span>🗑️</span>
          <span>Удалить</span>
        </button>
      </div>
    </div>

    <div class="content-grid">
  <div class="main-content">
    <div v-if="isLoading">
      <p>Загрузка данных дефекта...</p>
    </div>

    <div v-else-if="defect">
      <div class="defect-card">
        <div class="card-header">
          <div class="defect-title-section">
            <div class="defect-id">#{{ defect.id }}</div>
            <h1>{{ defect.title }}</h1>
          </div>
          <div class="priority-badge" :class="defect.priority">
            {{ getPriorityText(defect.priority) }}
          </div>
        </div>

        <div class="defect-meta">
          <div class="meta-item">
            <span class="meta-icon">📂</span>
            <div class="meta-content">
              <span class="meta-label">Проект</span>
              <span class="meta-value">{{ defect.project }}</span>
            </div>
          </div>
          
          <div class="meta-item full-width">
            <span class="meta-icon">👤</span>
            <div class="meta-content assignee-content">
              <span class="meta-label">Исполнитель</span>
              <AssigneeSelector
                :assignee="defect.assignee"
                :users="users"
                @assign="handleAssigneeChange"
                :disabled="!isEditing"
              />
            </div>
          </div>
          
          <div class="meta-item">
            <span class="meta-icon">📅</span>
            <div class="meta-content">
              <span class="meta-label">Дедлайн</span>
              <input
                v-if="isEditing"
                type="date"
                v-model="defect.deadline"
                class="meta-input"
                @change="onFieldChange('deadline')"
              />
              <span
                v-else
                class="meta-value"
                :class="{ 'overdue': isOverdue }"
                title="Нажмите, чтобы изменить"
              >
                {{ formatDate(defect.deadline) }}
              </span>
            </div>
          </div>
          
          <div class="meta-item">
            <span class="meta-icon">📊</span>
            <div class="meta-content">
              <span class="meta-label">Статус</span>
              <div class="status-badge" :class="getStatusClass(defect.status)">
                {{ defect.status }}
              </div>
            </div>
          </div>
        </div>

        <div class="description-section">
          <h3>Описание</h3>
          <p>{{ defect.description }}</p>
        </div>

        <div v-if="defect.photos && defect.photos.length > 0" class="photos-section">
          <h3>Фотографии</h3>
          <div class="photos-grid">
            <div 
              v-for="(photo, index) in defect.photos" 
              :key="index"
              class="photo-item"
              @click="openPhotoViewer(index)"
            >
              <img :src="photo" :alt="`Фото ${index + 1}`" />
            </div>
          </div>
        </div>
      </div>
      <div class="comments-card">
        <div class="comments-header">
          <h3>Комментарии</h3>
          <span class="comments-count">{{ comments.length }}</span>
        </div>
        </div>
      
    </div>
    <div v-else>
        <p>{{ error || 'Не удалось загрузить дефект.' }}</p>
    </div>
  </div>

  <div v-if="defect" class="sidebar">
    <div class="status-card">
      <h3>Изменить статус</h3>
      <div class="status-buttons">
        <button
          v-for="status in availableStatuses"
          :key="status.value"
          :class="['status-button', status.class, { active: defect.status === status.value }]"
          @click="changeStatus(status.value)"
          :disabled="!isEditing"
        >
          <span>{{ status.icon }}</span>
          <span>{{ status.label }}</span>
        </button>
      </div>
    </div>

    <div class="history-card">
      <h3>История изменений</h3>
      </div>
  </div>
</div>
    </div>

</template>

<script setup>
import axios from 'axios'
import { ref, computed, onMounted  } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useAuthStore } from '../store/auth';
import AssigneeSelector from '../components/AssigneeSelector.vue';

const defect = ref(null)
const isLoading = ref(true)
const error = ref(null)

const router = useRouter();
const route = useRoute();
const auth = useAuthStore();

const isEditing = ref(false)


onMounted(async () => {
  const { id } = route.params;

  // Если это режим создания, выходим (этот код вы уже исправляли)
  if (id === 'new' || id === undefined) {
    // ... (ваш код инициализации)
    return;
  }
  
  // Режим просмотра/редактирования: Загружаем данные
  try {
    const response = await axios.get(`/api/defects/${id}`);
    const rawDefect = response.data;
    
    // ⭐ ИСПРАВЛЕНИЕ: Объявляем переменные let в самом начале блока try
    let originalAssignedToId = null; 
    let originalProjectId = null;
    let assigneeObject = null;
    let projectTitle = 'Не назначен'; 
    
    // 1. ПОИСК ИСПОЛНИТЕЛЯ
    if (rawDefect.assignedToId) {
      // ⭐ Теперь эта переменная уже определена
      originalAssignedToId = rawDefect.assignedToId; 
      assigneeObject = users.value.find(user => user.id == originalAssignedToId) || null;
    }

    // 2. ПОИСК ПРОЕКТА
    if (rawDefect.projectId) {
      // ⭐ Теперь эта переменная уже определена
      originalProjectId = rawDefect.projectId; 
      
      const projectObject = projects.value.find(p => p.id == originalProjectId);
      
      if (projectObject) {
        projectTitle = projectObject.name;
      } else {
        projectTitle = `ID проекта: ${originalProjectId} (Неизвестен)`; 
      }
    }
    
    // Формируем объект defect.value
    defect.value = {
      id: rawDefect.id,
      title: rawDefect.title,        // 👈 ДОБАВЬТЕ ЭТО
      description: rawDefect.description,  // 👈 ДОБАВЬТЕ ЭТО
      priority: rawDefect.priority.toLowerCase(), // 👈 ДОБАВЬТЕ ЭТО (с преобразованием)
      status: rawDefect.status, 
      originalProjectId: originalProjectId, // 👈 Теперь она всегда определена (как null или ID)
      originalAssignedToId: originalAssignedToId, // 👈 Теперь она всегда определена
      createdAt: rawDefect.createdAt,
      project: projectTitle, 
      assignee: assigneeObject,
      deadline: rawDefect.dueDate,
    };

  } catch (err) {
    console.error('[DefectDetail] Load error:', err.message, err.response);
    error.value = 'Не удалось загрузить дефект.';
  } finally {
    isLoading.value = false
  }
})

const saveChanges = async () => {
    // ⭐ Защита от отсутствия данных
    if (!defect.value) {
        alert('Невозможно сохранить: данные дефекта отсутствуют.');
        return;
    }

    try {
        const id = defect.value.id;
        const isCreatingNew = id === null;
        // 1. Создаем объект, который ТОЧНО соответствует модели на сервере
        // ... внутри saveChanges
        const dataToSend = {
          id: id, 
          // 1. Title и Description: Защита от undefined
          title: defect.value.title || 'Новый дефект', 
          description: defect.value.description || '',
          
          // 2. Priority и Status: Защита от undefined/null
          priority: defect.value.priority || 'medium', 
          status: defect.value.status || 'New',
          createdAt: defect.value.createdAt, 
          // 3. ID (AssignedToId, ProjectId): Обеспечиваем, что это СТРОКА или null
          // (даже если бэкенд требует число, лучше явно преобразовать)
          assignedToId: defect.value.assignee?.id ? String(defect.value.assignee.id) : null, 
          projectId: defect.value.originalProjectId ? String(defect.value.originalProjectId) : null,

          // 4. ⭐ ДАТА: Самая важная часть! Преобразование в ISO-8601
          dueDate: defect.value.deadline 
              ? new Date(defect.value.deadline).toISOString() 
              : null,
          
          // 5. ID: Добавляем ID только для PUT-запроса (обновления)
          // ВАЖНО: При POST этот ID будет удален из finalDataToSend
           
          
          // 6. createdAt: Если сервер требует это поле, его нужно включить
          // createdAt: defect.value.createdAt || undefined,
        };
// ...

        // Определяем, это создание (POST) или обновление (PUT)
        
         let apiUrl;
        let requestPromise; // Переименуем, чтобы было понятнее
        let dataForApi = dataToSend;
        if (isCreatingNew) {
            // POST: Создание
            apiUrl = '/api/Defects';
            const { id: _, ...dataForPost } = dataToSend; // Удаляем ID
            dataForApi = dataForPost;
            requestPromise = axios.post(apiUrl, dataForApi);

        } else {
            // PUT: Обновление
            apiUrl = `/api/Defects/${id}`;
            requestPromise = axios.put(apiUrl, dataForApi); 
        }
        
        // ⭐ ЛОГИРОВАНИЕ: Выводятся корректные данные перед отправкой
        console.log('API URL:', apiUrl);
        console.log('Data to send:', dataForApi);
        
        const response = await requestPromise;

        isEditing.value = false;
        alert(isCreatingNew ? 'Дефект успешно создан!' : 'Изменения сохранены!');

        // Если создали, перенаправляем на страницу нового дефекта
        if (isCreatingNew) {
            router.push(`/defects/${response.data.id}`); 
        }

    } catch (err) {
        console.error('[DefectDetail] Save error:', err);
        // Добавьте alert для деталей ошибки, если она есть в теле ответа
        alert('Ошибка при сохранении! Проверьте консоль для деталей ответа сервера (AxiosError).');
    }
};


const newComment = ref('');

const projects = ref([
  { id: 1, name: 'CRM Система' },
  { id: 2, name: 'Веб-портал' },
  { id: 3, name: 'Мобильное приложение' },
  { id: 4, name: 'Аналитическая система' }
]);

const users = ref([
  { id: 1, name: 'Иван Петров', role: 'Старший инженер' },
  { id: 2, name: 'Мария Сидорова', role: 'Инженер' },
  { id: 3, name: 'Алексей Иванов', role: 'Младший инженер' },
  { id: 4, name: 'Дмитрий Смирнов', role: 'Менеджер проекта' },
  { id: 5, name: 'Екатерина Волкова', role: 'Инженер' },
  { id: 6, name: 'Сергей Козлов', role: 'Старший инженер' },
  { id: 7, name: 'Анна Морозова', role: 'Инженер' }
]);

const comments = ref([
  {
    id: 1,
    author: 'Мария Сидорова',
    text: 'Проверила логи, похоже проблема в Redis кэше. Иногда сессии не сохраняются.',
    timestamp: '2025-02-02T14:20:00'
  },
  {
    id: 2,
    author: 'Иван Петров',
    text: 'Спасибо за информацию! Начал работу над исправлением. Добавил дополнительную проверку для Redis.',
    timestamp: '2025-02-02T15:45:00'
  },
  {
    id: 3,
    author: 'Алексей Иванов',
    text: 'Можно также добавить fallback на in-memory хранилище, если Redis недоступен.',
    timestamp: '2025-02-03T09:10:00'
  }
]);

const history = ref([
  {
    action: 'Дефект создан',
    author: 'Мария Сидорова',
    timestamp: '2025-02-01T10:30:00',
    type: 'created'
  },
  {
    action: 'Назначен исполнитель',
    author: 'Дмитрий Смирнов',
    details: 'Иван Петров',
    timestamp: '2025-02-01T11:00:00',
    type: 'assigned'
  },
  {
    action: 'Статус изменён',
    author: 'Иван Петров',
    details: 'Открыт → В работе',
    timestamp: '2025-02-02T09:15:00',
    type: 'status'
  },
  {
    action: 'Приоритет изменён',
    author: 'Дмитрий Смирнов',
    details: 'Средний → Высокий',
    timestamp: '2025-02-02T14:00:00',
    type: 'priority'
  }
]);

const availableStatuses = [
  { value: 'Открыт', label: 'Открыт', icon: '🔴', class: 'status-open' },
  { value: 'В работе', label: 'В работе', icon: '🔵', class: 'status-progress' },
  { value: 'На проверке', label: 'На проверке', icon: '🟡', class: 'status-review' },
  { value: 'Закрыт', label: 'Закрыт', icon: '🟢', class: 'status-closed' }
];

const isOverdue = computed(() => {
  return new Date(defect.value.deadline) < new Date();
});

const goBack = () => {
  router.push('/defects');
};

const getPriorityText = (priority) => {
  const priorityTexts = {
    'high': 'Высокий',
    'medium': 'Средний',
    'low': 'Низкий'
  };
  return priorityTexts[priority] || priority;
};

const getStatusClass = (status) => {
  const statusClasses = {
    'Открыт': 'status-open',
    'В работе': 'status-progress',
    'На проверке': 'status-review',
    'Закрыт': 'status-closed'
  };
  return statusClasses[status] || 'status-default';
};

const formatDate = (dateString) => {
  const date = new Date(dateString);
  return date.toLocaleDateString('ru-RU', { 
    day: 'numeric', 
    month: 'long', 
    year: 'numeric' 
  });
};

const onFieldChange = (field) => {
  history.value.push({
    action: `Изменено поле "${field}"`,
    author: auth.user?.fullName || 'Пользователь',
    timestamp: new Date().toISOString(),
    details: `Новое значение: ${defect.value[field]}`,
    type: 'edit'
  });
}

const formatTime = (timestamp) => {
  const date = new Date(timestamp);
  const now = new Date();
  const diffMs = now - date;
  const diffMins = Math.floor(diffMs / 60000);
  const diffHours = Math.floor(diffMs / 3600000);
  const diffDays = Math.floor(diffMs / 86400000);

  if (diffMins < 1) return 'только что';
  if (diffMins < 60) return `${diffMins} мин назад`;
  if (diffHours < 24) return `${diffHours} ч назад`;
  if (diffDays < 7) return `${diffDays} дн назад`;
  
  return date.toLocaleDateString('ru-RU', { 
    day: 'numeric', 
    month: 'short' 
  });
};

const getUserInitials = () => {
  const user = auth.user;
  if (!user || !user.username) return '?';
  return user.username.substring(0, 2).toUpperCase();
};

const addComment = () => {
  if (!newComment.value.trim()) return;
  
  const comment = {
    id: comments.value.length + 1,
    author: auth.user?.fullName || auth.user?.username || 'Пользователь',
    text: newComment.value,
    timestamp: new Date().toISOString()
  };
  
  comments.value.push(comment);
  newComment.value = '';
  
  history.value.push({
    action: 'Добавлен комментарий',
    author: comment.author,
    timestamp: comment.timestamp,
    type: 'comment'
  });
};

const changeStatus = (newStatus) => {
  if ( defect.value.status === newStatus) return;
  
  const oldStatus = defect.value.status;
  defect.value.status = newStatus;
  
  history.value.push({
    action: 'Статус изменён',
    author: auth.user?.fullName || auth.user?.username || 'Пользователь',
    details: `${oldStatus} → ${newStatus}`,
    timestamp: new Date().toISOString(),
    type: 'status'
  });
  
  console.log('[v0] Status changed:', { oldStatus, newStatus });
};

const handleAssigneeChange = (newAssignee) => {
  const oldAssignee = defect.value.assignee;
  defect.value.assignee = newAssignee;
  
  const oldName = oldAssignee?.name || 'Не назначен';
  const newName = newAssignee?.name || 'Не назначен';
  
  history.value.push({
    action: 'Исполнитель изменён',
    author: auth.user?.fullName || auth.user?.username || 'Пользователь',
    details: `${oldName} → ${newName}`,
    timestamp: new Date().toISOString(),
    type: 'assigned'
  });
  
  console.log('[v0] Assignee changed:', { oldAssignee, newAssignee });
};

const handleDelete = async () => {
  if (!confirm('Удалить этот дефект?')) return
  try {
    const { id } = defect.value
    await axios.delete(`/api/defects/${id}`)
    router.push('/defects')
  } catch (err) {
    console.error('[DefectDetail] Delete error:', err)
    alert('Ошибка при удалении!')
  }
}


const openPhotoViewer = (index) => {
  console.log('[v0] Opening photo viewer:', index);
};

const canEdit = computed(() => true);
const canDelete = computed(() => true);
const canChangeStatus = computed(() => true);
</script>

<style scoped>
.defect-detail-page {
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.back-button {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  color: #2d3748;
  padding: 0.75rem 1.25rem;
  border: none;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.back-button:hover {
  transform: translateX(-5px);
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.15);
}

.header-actions {
  display: flex;
  gap: 0.75rem;
}

.action-button {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.25rem;
  border: none;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.action-button.edit {
  background: rgba(255, 255, 255, 0.95);
  color: #667eea;
}

.action-button.edit:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(102, 126, 234, 0.3);
}

.action-button.delete {
  background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
  color: white;
}

.action-button.delete:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(239, 68, 68, 0.4);
}

.content-grid {
  display: grid;
  grid-template-columns: 1fr 350px;
  gap: 1.5rem;
}

.main-content {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.defect-card,
.comments-card,
.status-card,
.history-card {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  border-radius: 20px;
  padding: 2rem;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 1.5rem;
  padding-bottom: 1.5rem;
  border-bottom: 1px solid #e2e8f0;
}

.defect-title-section {
  flex: 1;
}

.defect-id {
  font-family: monospace;
  font-size: 0.9rem;
  color: #718096;
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.defect-card h1 {
  font-size: 2rem;
  font-weight: 700;
  color: #2d3748;
  margin: 0;
  line-height: 1.3;
}

.priority-badge {
  padding: 0.5rem 1rem;
  border-radius: 12px;
  font-weight: 600;
  font-size: 0.9rem;
  white-space: nowrap;
}

.priority-badge.high {
  background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
  color: white;
}

.priority-badge.medium {
  background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
  color: white;
}

.priority-badge.low {
  background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);
  color: white;
}

.defect-meta {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.meta-item {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.meta-item.full-width {
  grid-column: 1 / -1;
}

.meta-icon {
  font-size: 1.75rem;
  width: 48px;
  height: 48px;
  background: #f7fafc;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.meta-content {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.meta-content.assignee-content {
  flex: 1;
  gap: 0.5rem;
}

.meta-label {
  font-size: 0.85rem;
  color: #718096;
  font-weight: 500;
}

.meta-value {
  font-size: 1rem;
  color: #2d3748;
  font-weight: 600;
}

.meta-value.overdue {
  color: #ef4444;
}

.status-badge {
  display: inline-block;
  padding: 0.35rem 0.75rem;
  border-radius: 8px;
  font-weight: 600;
  font-size: 0.85rem;
}

.status-badge.status-open {
  background: #fee2e2;
  color: #dc2626;
}

.status-badge.status-progress {
  background: #dbeafe;
  color: #2563eb;
}

.status-badge.status-review {
  background: #fef3c7;
  color: #d97706;
}

.status-badge.status-closed {
  background: #d1fae5;
  color: #059669;
}

.description-section,
.photos-section {
  margin-bottom: 2rem;
}

.description-section h3,
.photos-section h3 {
  font-size: 1.25rem;
  font-weight: 700;
  color: #2d3748;
  margin: 0 0 1rem;
}

.description-section p {
  color: #4a5568;
  line-height: 1.7;
  margin: 0;
}

.photos-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 1rem;
}

.photo-item {
  aspect-ratio: 4/3;
  border-radius: 12px;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.photo-item:hover {
  transform: scale(1.05);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.15);
}

.photo-item img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.comments-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.comments-header h3 {
  font-size: 1.25rem;
  font-weight: 700;
  color: #2d3748;
  margin: 0;
}

.comments-count {
  background: #f7fafc;
  color: #718096;
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-weight: 600;
  font-size: 0.9rem;
}

.add-comment-form {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  padding-bottom: 2rem;
  border-bottom: 1px solid #e2e8f0;
}

.user-avatar {
  width: 40px;
  height: 40px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 600;
  flex-shrink: 0;
}

.comment-input-wrapper {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.comment-input {
  width: 100%;
  padding: 0.875rem 1rem;
  background: #f7fafc;
  border: 2px solid #e2e8f0;
  border-radius: 12px;
  color: #2d3748;
  font-size: 0.95rem;
  font-family: inherit;
  resize: vertical;
  outline: none;
  transition: all 0.3s ease;
}

.comment-input:focus {
  background: white;
  border-color: #667eea;
  box-shadow: 0 0 0 4px rgba(102, 126, 234, 0.1);
}

.send-button {
  align-self: flex-end;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 10px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
}

.send-button:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(102, 126, 234, 0.4);
}

.send-button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.comments-list {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.comment-item {
  display: flex;
  gap: 1rem;
}

.comment-avatar {
  width: 36px;
  height: 36px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 600;
  font-size: 0.9rem;
  flex-shrink: 0;
}

.comment-content {
  flex: 1;
}

.comment-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.comment-author {
  font-weight: 600;
  color: #2d3748;
}

.comment-time {
  font-size: 0.85rem;
  color: #718096;
}

.comment-text {
  color: #4a5568;
  line-height: 1.6;
  margin: 0;
}

.empty-comments {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 3rem 1rem;
  text-align: center;
}

.empty-icon {
  font-size: 3rem;
  margin-bottom: 0.75rem;
  opacity: 0.5;
}

.empty-comments p {
  color: #718096;
  margin: 0;
}

.sidebar {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.status-card h3,
.history-card h3 {
  font-size: 1.1rem;
  font-weight: 700;
  color: #2d3748;
  margin: 0 0 1.25rem;
}

.status-buttons {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.status-button {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.875rem 1rem;
  background: #f7fafc;
  border: 2px solid #e2e8f0;
  border-radius: 12px;
  color: #4a5568;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.status-button:hover:not(:disabled) {
  background: white;
  transform: translateX(5px);
}

.status-button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.status-button.active {
  border-color: #667eea;
  background: linear-gradient(135deg, rgba(102, 126, 234, 0.1) 0%, rgba(118, 75, 162, 0.1) 100%);
  color: #667eea;
}

.timeline {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.timeline-item {
  display: flex;
  gap: 1rem;
  position: relative;
}

.timeline-item:not(:last-child)::before {
  content: '';
  position: absolute;
  left: 11px;
  top: 30px;
  bottom: -24px;
  width: 2px;
  background: #e2e8f0;
}

.timeline-dot {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  flex-shrink: 0;
  position: relative;
  z-index: 1;
}

.timeline-dot.created {
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
}

.timeline-dot.assigned {
  background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);
}

.timeline-dot.status {
  background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
}

.timeline-dot.priority {
  background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
}

.timeline-dot.comment {
  background: linear-gradient(135deg, #8b5cf6 0%, #7c3aed 100%);
}

.timeline-content {
  flex: 1;
}

.timeline-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.25rem;
}

.timeline-action {
  font-weight: 600;
  color: #2d3748;
  font-size: 0.95rem;
}

.timeline-time {
  font-size: 0.8rem;
  color: #718096;
}

.timeline-author {
  color: #718096;
  font-size: 0.9rem;
  margin: 0 0 0.25rem;
}

.timeline-details {
  color: #4a5568;
  font-size: 0.9rem;
  margin: 0;
  font-weight: 500;
}

@media (max-width: 1024px) {
  .content-grid {
    grid-template-columns: 1fr;
  }
  
  .sidebar {
    order: -1;
  }
}

@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    align-items: stretch;
    gap: 1rem;
  }
  
  .header-actions {
    justify-content: stretch;
  }
  
  .action-button {
    flex: 1;
  }
  
  .defect-meta {
    grid-template-columns: 1fr;
  }
  
  .photos-grid {
    grid-template-columns: 1fr;
  }
}
</style>
