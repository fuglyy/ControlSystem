<template>
  <div class="projects-page">
    <div class="page-header">
      <div class="header-content">
        <div class="header-icon">📂</div>
        <div>
          <h1>Проекты</h1>
          <p class="header-subtitle">Управление всеми проектами</p>
        </div>
      </div>
      <button class="create-button" @click="showAddModal = true">
        <span class="button-icon">✨</span>
        <span>Создать проект</span>
        <div class="button-shine"></div>
      </button>
    </div>
    
    <div class="filters-bar">
      <div class="search-box">
        <span class="search-icon">🔍</span>
        <input 
          v-model="searchQuery" 
          type="text" 
          placeholder="Найти проект..." 
          class="search-input"
        />
      </div>
      
      <div class="filter-pills">
        <button 
          v-for="status in statuses" 
          :key="status.value"
          :class="['filter-pill', { active: statusFilter === status.value }]"
          @click="statusFilter = status.value"
        >
          <span>{{ status.icon }}</span>
          <span>{{ status.label }}</span>
        </button>
      </div>
    </div>
    
    <div class="projects-grid">
      <div
        v-for="project in filteredProjects"
        :key="project.id"
        class="project-card"
        @click="viewProject(project)"
      >
        <div class="card-background" :style="{ background: project.gradient }"></div>
        
        <div class="card-content">
          <div class="project-header">
            <div class="project-icon" :style="{ background: project.gradient }">
              {{ project.icon }}
            </div>
            <div class="project-status" :class="getStatusClass(project.status)">
              <span class="status-dot"></span>
              <span>{{ project.status }}</span>
            </div>
          </div>
          
          <h3>{{ project.name }}</h3>
          <p class="project-description">{{ project.description }}</p>
          
          <div class="project-stats">
            <div class="stat-item">
              <span class="stat-icon">✓</span>
              <span>{{ project.completed }}/{{ project.total }}</span>
            </div>
            <div class="stat-item">
              <span class="stat-icon">👥</span>
              <span>{{ project.members.length }}</span>
            </div>
            <div class="stat-item">
              <span class="stat-icon">📅</span>
              <span>{{ project.deadline }}</span>
            </div>
          </div>
          
          <div class="progress-section">
            <div class="progress-header">
              <span>Прогресс</span>
              <span class="progress-percent">{{ project.progress }}%</span>
            </div>
            <div class="progress-bar">
              <div 
                class="progress-fill" 
                :style="{ 
                  width: project.progress + '%',
                  background: project.gradient 
                }"
              ></div>
            </div>
          </div>
          
          <div class="card-footer">
            <div class="members-avatars">
              <div 
                v-for="(member, index) in project.members.slice(0, 3)" 
                :key="index"
                class="member-avatar"
                :style="{ zIndex: 10 - index }"
              >
                {{ member.charAt(0) }}
              </div>
              <div 
                v-if="project.members.length > 3" 
                class="member-avatar more"
              >
                +{{ project.members.length - 3 }}
              </div>
            </div>
            <button class="card-action-btn" @click.stop="editProject(project)">
              <span>⚙️</span>
            </button>
          </div>
        </div>
      </div>
    </div>
    
    <div v-if="filteredProjects.length === 0" class="empty-state">
      <div class="empty-illustration">
        <div class="empty-circle"></div>
        <div class="empty-icon">📂</div>
      </div>
      <h3>Проекты не найдены</h3>
      <p>Попробуйте изменить фильтры или создайте новый проект</p>
      <button class="empty-action-btn" @click="showAddModal = true">
        <span>✨</span>
        <span>Создать первый проект</span>
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';

const searchQuery = ref('');
const statusFilter = ref('');
const showAddModal = ref(false);

const statuses = [
  { value: '', label: 'Все', icon: '📋' },
  { value: 'В работе', label: 'В работе', icon: '⚡' },
  { value: 'Завершён', label: 'Завершён', icon: '✓' },
  { value: 'Запланирован', label: 'Запланирован', icon: '📅' }
];

const projects = ref([
  {
    id: 1,
    name: 'CRM Система',
    description: 'Внутренний инструмент для управления клиентами и продажами.',
    status: 'В работе',
    members: ['Иван', 'Мария', 'Алексей'],
    icon: '💼',
    gradient: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
    progress: 75,
    completed: 15,
    total: 20,
    deadline: '15 дек'
  },
  {
    id: 2,
    name: 'Мобильное приложение',
    description: 'Кроссплатформенное приложение для Android и iOS.',
    status: 'Завершён',
    members: ['Ольга', 'Дмитрий'],
    icon: '📱',
    gradient: 'linear-gradient(135deg, #48bb78 0%, #38a169 100%)',
    progress: 100,
    completed: 25,
    total: 25,
    deadline: '1 дек'
  },
  {
    id: 3,
    name: 'Веб-портал',
    description: 'Сайт для заказчиков с личным кабинетом.',
    status: 'Запланирован',
    members: ['Светлана', 'Кирилл', 'Анна', 'Павел'],
    icon: '🌐',
    gradient: 'linear-gradient(135deg, #4299e1 0%, #3182ce 100%)',
    progress: 30,
    completed: 6,
    total: 20,
    deadline: '20 дек'
  },
  {
    id: 4,
    name: 'Аналитическая система',
    description: 'Инструмент для анализа данных и составления отчетов.',
    status: 'В работе',
    members: ['Дмитрий', 'Екатерина'],
    icon: '📊',
    gradient: 'linear-gradient(135deg, #ed8936 0%, #dd6b20 100%)',
    progress: 45,
    completed: 9,
    total: 20,
    deadline: '10 янв'
  }
]);

const filteredProjects = computed(() => {
  return projects.value.filter(project => {
    const matchesSearch = !searchQuery.value || 
      project.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      project.description.toLowerCase().includes(searchQuery.value.toLowerCase());
    
    const matchesStatus = !statusFilter.value || project.status === statusFilter.value;
    
    return matchesSearch && matchesStatus;
  });
});

const getStatusClass = (status) => {
  const statusClasses = {
    'В работе': 'status-progress',
    'Завершён': 'status-completed',
    'Запланирован': 'status-planned'
  };
  return statusClasses[status] || 'status-default';
};

const viewProject = (project) => {
  console.log('Просмотр проекта:', project);
};

const editProject = (project) => {
  console.log('Редактирование проекта:', project);
};
</script>

<style scoped>
.projects-page {
  color: white;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  animation: fadeInDown 0.6s ease-out;
}

@keyframes fadeInDown {
  from {
    opacity: 0;
    transform: translateY(-20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.header-content {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.header-icon {
  width: 70px;
  height: 70px;
  background: white;
  border-radius: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 2.5rem;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
  animation: bounce 2s ease-in-out infinite;
}

.page-header h1 {
  font-size: 2.5rem;
  font-weight: 800;
  color: white;
  margin: 0;
  text-shadow: 0 2px 20px rgba(0, 0, 0, 0.2);
}

.header-subtitle {
  color: rgba(255, 255, 255, 0.9);
  font-size: 1.1rem;
  margin: 0.25rem 0 0;
}

.create-button {
  position: relative;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  background: white;
  color: #667eea;
  padding: 1rem 2rem;
  border: none;
  border-radius: 16px;
  font-weight: 700;
  font-size: 1.05rem;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
  overflow: hidden;
}

.create-button:hover {
  transform: translateY(-3px);
  box-shadow: 0 15px 40px rgba(0, 0, 0, 0.3);
}

.button-icon {
  font-size: 1.5rem;
}

.button-shine {
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.5), transparent);
  transition: left 0.5s;
}

.create-button:hover .button-shine {
  left: 100%;
}

.filters-bar {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  border-radius: 20px;
  padding: 1.5rem;
  margin-bottom: 2rem;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
  animation: fadeIn 0.6s ease-out 0.2s both;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

.search-box {
  position: relative;
  margin-bottom: 1.5rem;
}

.search-icon {
  position: absolute;
  left: 1.25rem;
  top: 50%;
  transform: translateY(-50%);
  font-size: 1.25rem;
}

.search-input {
  width: 100%;
  padding: 1rem 1rem 1rem 3.5rem;
  background: #f7fafc;
  border: 2px solid transparent;
  border-radius: 14px;
  color: #2d3748;
  font-size: 1rem;
  outline: none;
  transition: all 0.3s ease;
}

.search-input:focus {
  background: white;
  border-color: #667eea;
  box-shadow: 0 0 0 4px rgba(102, 126, 234, 0.1);
}

.filter-pills {
  display: flex;
  gap: 0.75rem;
  flex-wrap: wrap;
}

.filter-pill {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.25rem;
  background: #f7fafc;
  border: 2px solid transparent;
  border-radius: 12px;
  color: #4a5568;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.filter-pill:hover {
  background: white;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.filter-pill.active {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-color: transparent;
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4);
}

.projects-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 1.5rem;
  animation: fadeIn 0.6s ease-out 0.4s both;
}

.project-card {
  position: relative;
  background: white;
  border-radius: 20px;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
}

.project-card:hover {
  transform: translateY(-10px) scale(1.02);
  box-shadow: 0 20px 50px rgba(0, 0, 0, 0.2);
}

.card-background {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 140px;
  opacity: 0.15;
  transition: all 0.4s ease;
}

.project-card:hover .card-background {
  opacity: 0.25;
  height: 160px;
}

.card-content {
  position: relative;
  padding: 1.75rem;
}

.project-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.25rem;
}

.project-icon {
  width: 60px;
  height: 60px;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 2rem;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.15);
  transition: all 0.3s ease;
}

.project-card:hover .project-icon {
  transform: scale(1.1) rotate(-5deg);
}

.project-status {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.875rem;
  font-weight: 600;
}

.status-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: currentColor;
  animation: pulse 2s ease-in-out infinite;
}

.status-progress {
  background: #bee3f8;
  color: #3182ce;
}

.status-completed {
  background: #c6f6d5;
  color: #38a169;
}

.status-planned {
  background: #feebc8;
  color: #dd6b20;
}

.project-card h3 {
  font-size: 1.5rem;
  font-weight: 700;
  color: #2d3748;
  margin: 0 0 0.75rem;
}

.project-description {
  color: #718096;
  font-size: 0.95rem;
  line-height: 1.6;
  margin: 0 0 1.5rem;
}

.project-stats {
  display: flex;
  gap: 1.5rem;
  margin-bottom: 1.5rem;
}

.stat-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #718096;
  font-size: 0.9rem;
  font-weight: 500;
}

.stat-icon {
  font-size: 1.1rem;
}

.progress-section {
  margin-bottom: 1.5rem;
}

.progress-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 0.75rem;
  font-size: 0.9rem;
  color: #718096;
}

.progress-percent {
  font-weight: 700;
  color: #2d3748;
}

.progress-bar {
  height: 10px;
  background: #edf2f7;
  border-radius: 10px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  border-radius: 10px;
  transition: width 0.6s ease;
  position: relative;
  overflow: hidden;
}

.progress-fill::after {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.3), transparent);
  animation: shimmer 2s infinite;
}

@keyframes shimmer {
  0% { transform: translateX(-100%); }
  100% { transform: translateX(100%); }
}

.card-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: 1.25rem;
  border-top: 1px solid #e2e8f0;
}

.members-avatars {
  display: flex;
  align-items: center;
}

.member-avatar {
  width: 36px;
  height: 36px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 600;
  font-size: 0.875rem;
  border: 3px solid white;
  margin-left: -10px;
  transition: all 0.3s ease;
}

.member-avatar:first-child {
  margin-left: 0;
}

.member-avatar:hover {
  transform: translateY(-5px) scale(1.1);
  z-index: 100 !important;
}

.member-avatar.more {
  background: #cbd5e0;
  color: #4a5568;
  font-size: 0.75rem;
}

.card-action-btn {
  width: 40px;
  height: 40px;
  background: #f7fafc;
  border: none;
  border-radius: 10px;
  font-size: 1.25rem;
  cursor: pointer;
  transition: all 0.3s ease;
}

.card-action-btn:hover {
  background: #edf2f7;
  transform: rotate(90deg);
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 5rem 2rem;
  text-align: center;
  animation: fadeIn 0.6s ease-out;
}

.empty-illustration {
  position: relative;
  width: 150px;
  height: 150px;
  margin-bottom: 2rem;
}

.empty-circle {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 100%;
  height: 100%;
  background: white;
  border-radius: 50%;
  opacity: 0.3;
  animation: ping 2s cubic-bezier(0, 0, 0.2, 1) infinite;
}

@keyframes ping {
  75%, 100% {
    transform: translate(-50%, -50%) scale(1.5);
    opacity: 0;
  }
}

.empty-icon {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  font-size: 4rem;
  animation: float 3s ease-in-out infinite;
}

.empty-state h3 {
  font-size: 1.75rem;
  color: white;
  margin: 0 0 0.75rem;
}

.empty-state p {
  color: rgba(255, 255, 255, 0.8);
  font-size: 1.1rem;
  margin: 0 0 2rem;
  max-width: 400px;
}

.empty-action-btn {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  background: white;
  color: #667eea;
  padding: 1rem 2rem;
  border: none;
  border-radius: 14px;
  font-weight: 700;
  font-size: 1.05rem;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
}

.empty-action-btn:hover {
  transform: translateY(-3px);
  box-shadow: 0 15px 40px rgba(0, 0, 0, 0.3);
}

@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }
  
  .header-content {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }
  
  .projects-grid {
    grid-template-columns: 1fr;
  }
}
</style>
