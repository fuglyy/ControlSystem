<template>
  <div class="dashboard-page">
    <div class="page-header">
      <div>
        <h1>Добро пожаловать!</h1>
        <p class="header-subtitle">Вот что происходит в ваших проектах сегодня</p>
      </div>
      <button class="primary-button">
        <span>➕</span>
        <span>Создать проект</span>
      </button>
    </div>
    
    <div class="stats-grid">
      <div class="stat-card" v-for="stat in stats" :key="stat.id">
        <div class="stat-header">
          <div class="stat-icon" :style="{ background: stat.color }">
            {{ stat.icon }}
          </div>
          <div class="stat-trend" :class="stat.trendType">
            <span class="trend-arrow">{{ stat.trendType === 'up' ? '↗' : '↘' }}</span>
            <span>{{ stat.trend }}</span>
          </div>
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ stat.value }}</div>
          <div class="stat-label">{{ stat.label }}</div>
        </div>
      </div>
    </div>
    
    <div class="dashboard-grid">
      <div class="dashboard-card">
        <div class="card-header">
          <div>
            <h2>Активные проекты</h2>
            <p class="card-subtitle">Ваши текущие задачи</p>
          </div>
          <button class="view-all-link" @click="$router.push('/projects')">
            Все проекты →
          </button>
        </div>
        <div class="recent-projects">
          <div 
            v-for="project in recentProjects" 
            :key="project.id"
            class="project-item"
          >
            <div class="project-header">
              <div class="project-icon" :style="{ background: project.color }">
                {{ project.icon }}
              </div>
              <div class="project-info">
                <div class="project-name">{{ project.name }}</div>
                <div class="project-team">{{ project.team }} участников</div>
              </div>
            </div>
            <div class="project-progress">
              <div class="progress-info">
                <span>Прогресс</span>
                <span class="progress-percent">{{ project.progress }}%</span>
              </div>
              <div class="progress-bar">
                <div 
                  class="progress-fill" 
                  :style="{ width: project.progress + '%', background: project.color }"
                ></div>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <div class="dashboard-card">
        <div class="card-header">
          <div>
            <h2>Последние дефекты</h2>
            <p class="card-subtitle">Требуют внимания</p>
          </div>
          <button class="view-all-link" @click="$router.push('/defects')">
            Все дефекты →
          </button>
        </div>
        <div class="defects-list">
          <div 
            v-for="defect in activeDefects" 
            :key="defect.id"
            class="defect-item"
          >
            <div class="defect-priority-badge" :class="defect.priority">
              <span class="priority-dot"></span>
              {{ getPriorityText(defect.priority) }}
            </div>
            <div class="defect-content">
              <div class="defect-title">{{ defect.title }}</div>
              <div class="defect-meta">
                <span>{{ defect.project }}</span>
                <span class="meta-dot">•</span>
                <span>{{ defect.assignee }}</span>
              </div>
            </div>
            <div class="defect-status" :class="defect.statusClass">
              {{ defect.status }}
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <div class="activity-card">
      <div class="card-header">
        <div>
          <h2>Лента активности</h2>
          <p class="card-subtitle">Последние обновления</p>
        </div>
      </div>
      <div class="activity-timeline">
        <div 
          v-for="activity in activities" 
          :key="activity.id"
          class="activity-item"
        >
          <div class="activity-icon" :style="{ background: activity.color }">
            {{ activity.icon }}
          </div>
          <div class="activity-content">
            <div class="activity-text">{{ activity.text }}</div>
            <div class="activity-time">{{ activity.time }}</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const stats = ref([
  {
    id: 1,
    icon: '📂',
    label: 'Всего проектов',
    value: '12',
    trend: '+2',
    trendType: 'up',
    color: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)'
  },
  {
    id: 2,
    icon: '✓',
    label: 'Завершённых',
    value: '248',
    trend: '+15',
    trendType: 'up',
    color: 'linear-gradient(135deg, #48bb78 0%, #38a169 100%)'
  },
  {
    id: 3,
    icon: '🐛',
    label: 'Дефекты',
    value: '7',
    trend: '-3',
    trendType: 'down',
    color: 'linear-gradient(135deg, #ed8936 0%, #dd6b20 100%)'
  },
  {
    id: 4,
    icon: '👥',
    label: 'Участники',
    value: '24',
    trend: '+4',
    trendType: 'up',
    color: 'linear-gradient(135deg, #4299e1 0%, #3182ce 100%)'
  }
]);

const recentProjects = ref([
  { 
    id: 1, 
    name: 'CRM Система', 
    team: 3,
    progress: 75,
    icon: '💼',
    color: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)'
  },
  { 
    id: 2, 
    name: 'Мобильное приложение', 
    team: 5,
    progress: 100,
    icon: '📱',
    color: 'linear-gradient(135deg, #48bb78 0%, #38a169 100%)'
  },
  { 
    id: 3, 
    name: 'Веб-портал', 
    team: 4,
    progress: 45,
    icon: '🌐',
    color: 'linear-gradient(135deg, #4299e1 0%, #3182ce 100%)'
  }
]);

const activeDefects = ref([
  {
    id: 1,
    title: 'Ошибка в форме логина',
    project: 'CRM Система',
    assignee: 'Иван',
    status: 'Открыт',
    statusClass: 'status-open',
    priority: 'high'
  },
  {
    id: 2,
    title: 'Не грузится отчет',
    project: 'Веб-портал',
    assignee: 'Мария',
    status: 'В работе',
    statusClass: 'status-progress',
    priority: 'medium'
  },
  {
    id: 3,
    title: 'Проблема с авторизацией',
    project: 'Мобильное приложение',
    assignee: 'Алексей',
    status: 'Проверка',
    statusClass: 'status-review',
    priority: 'low'
  }
]);

const activities = ref([
  {
    id: 1,
    icon: '✓',
    text: 'Иван завершил задачу "Разработка API"',
    time: '2 минуты назад',
    color: 'linear-gradient(135deg, #48bb78 0%, #38a169 100%)'
  },
  {
    id: 2,
    icon: '💬',
    text: 'Новый комментарий к дефекту #42',
    time: '15 минут назад',
    color: 'linear-gradient(135deg, #4299e1 0%, #3182ce 100%)'
  },
  {
    id: 3,
    icon: '📂',
    text: 'Мария создала новый проект "Аналитическая панель"',
    time: '1 час назад',
    color: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)'
  },
  {
    id: 4,
    icon: '🐛',
    text: 'Алексей сообщил о новом дефекте',
    time: '2 часа назад',
    color: 'linear-gradient(135deg, #ed8936 0%, #dd6b20 100%)'
  }
]);

const getPriorityText = (priority) => {
  const priorityTexts = {
    'high': 'Высокий',
    'medium': 'Средний',
    'low': 'Низкий'
  };
  return priorityTexts[priority] || priority;
};
</script>

<style scoped>
.dashboard-page {
  color: white;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.page-header h1 {
  font-size: 2.5rem;
  font-weight: 700;
  color: white;
  margin-bottom: 0.5rem;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.header-subtitle {
  color: rgba(255, 255, 255, 0.8);
  font-size: 1.1rem;
}

.primary-button {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: white;
  color: #667eea;
  padding: 0.875rem 1.5rem;
  border: none;
  border-radius: 12px;
  font-weight: 600;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.2s ease;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
}

.primary-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 25px rgba(0, 0, 0, 0.2);
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.stat-card {
  background: white;
  border-radius: 16px;
  padding: 1.5rem;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
}

.stat-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.12);
}

.stat-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.stat-icon {
  width: 50px;
  height: 50px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
}

.stat-trend {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.375rem 0.75rem;
  border-radius: 20px;
  font-size: 0.875rem;
  font-weight: 600;
}

.stat-trend.up {
  background: #d6f5e3;
  color: #38a169;
}

.stat-trend.down {
  background: #fed7d7;
  color: #e53e3e;
}

.trend-arrow {
  font-size: 1rem;
}

.stat-value {
  font-size: 2.25rem;
  font-weight: 700;
  color: #2d3748;
  margin-bottom: 0.25rem;
}

.stat-label {
  color: #718096;
  font-size: 0.95rem;
}

.dashboard-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(450px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.dashboard-card,
.activity-card {
  background: white;
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid #e2e8f0;
}

.card-header h2 {
  font-size: 1.25rem;
  font-weight: 700;
  color: #2d3748;
  margin: 0 0 0.25rem;
}

.card-subtitle {
  color: #718096;
  font-size: 0.875rem;
  margin: 0;
}

.view-all-link {
  background: none;
  border: none;
  color: #667eea;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  padding: 0;
}

.view-all-link:hover {
  transform: translateX(5px);
}

.recent-projects,
.defects-list {
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.project-item {
  padding: 1.25rem;
  background: #f7fafc;
  border-radius: 12px;
  transition: all 0.2s;
}

.project-item:hover {
  background: #edf2f7;
  transform: translateX(4px);
}

.project-header {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1rem;
}

.project-icon {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
}

.project-info {
  flex: 1;
}

.project-name {
  font-weight: 600;
  font-size: 1.05rem;
  color: #2d3748;
  margin-bottom: 0.25rem;
}

.project-team {
  font-size: 0.875rem;
  color: #718096;
}

.project-progress {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.progress-info {
  display: flex;
  justify-content: space-between;
  font-size: 0.875rem;
  color: #718096;
}

.progress-percent {
  font-weight: 600;
  color: #2d3748;
}

.progress-bar {
  height: 8px;
  background: #e2e8f0;
  border-radius: 4px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  border-radius: 4px;
  transition: width 0.3s ease;
}

.defect-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1.25rem;
  background: #f7fafc;
  border-radius: 12px;
  transition: all 0.2s;
}

.defect-item:hover {
  background: #edf2f7;
}

.defect-priority-badge {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.875rem;
  font-weight: 600;
  white-space: nowrap;
}

.defect-priority-badge.high {
  background: #fed7d7;
  color: #e53e3e;
}

.defect-priority-badge.medium {
  background: #feebc8;
  color: #dd6b20;
}

.defect-priority-badge.low {
  background: #bee3f8;
  color: #3182ce;
}

.priority-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: currentColor;
}

.defect-content {
  flex: 1;
}

.defect-title {
  font-weight: 600;
  color: #2d3748;
  margin-bottom: 0.25rem;
}

.defect-meta {
  font-size: 0.875rem;
  color: #718096;
  display: flex;
  gap: 0.5rem;
}

.meta-dot {
  color: #cbd5e0;
}

.defect-status {
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.875rem;
  font-weight: 600;
  white-space: nowrap;
}

.status-open {
  background: #fed7d7;
  color: #e53e3e;
}

.status-progress {
  background: #bee3f8;
  color: #3182ce;
}

.status-review {
  background: #feebc8;
  color: #dd6b20;
}

.activity-timeline {
  padding: 1.5rem;
}

.activity-item {
  display: flex;
  gap: 1rem;
  padding: 1rem 0;
  border-left: 2px solid #e2e8f0;
  margin-left: 1.5rem;
  position: relative;
}

.activity-item:before {
  content: '';
  position: absolute;
  left: -6px;
  top: 1rem;
  width: 10px;
  height: 10px;
  background: white;
  border: 2px solid #667eea;
  border-radius: 50%;
}

.activity-icon {
  width: 40px;
  height: 40px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.25rem;
  flex-shrink: 0;
}

.activity-content {
  flex: 1;
  padding-top: 0.25rem;
}

.activity-text {
  color: #2d3748;
  margin-bottom: 0.25rem;
}

.activity-time {
  font-size: 0.875rem;
  color: #718096;
}

@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }
  
  .stats-grid {
    grid-template-columns: 1fr;
  }
  
  .dashboard-grid {
    grid-template-columns: 1fr;
  }
}
</style>
