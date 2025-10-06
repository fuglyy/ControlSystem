<template>
  <div class="reports-page">
    <div class="page-header">
      <div class="header-content">
        <h1>📈 Отчёты</h1>
        <p class="header-subtitle">Аналитика и статистика проектов</p>
      </div>
      <button v-if="auth.user?.role === 'Manager'" class="generate-button">
        <span class="button-icon">📊</span>
        <span>Создать отчёт</span>
      </button>
    </div>
    
    <div class="reports-grid">
      <div class="report-card" v-for="report in reports" :key="report.id">
        <div class="report-icon" :style="{ background: report.color }">
          {{ report.icon }}
        </div>
        <div class="report-content">
          <h3>{{ report.title }}</h3>
          <p>{{ report.description }}</p>
          <button class="view-button" @click="viewReport(report)">
            Просмотреть →
          </button>
        </div>
      </div>
    </div>
    
    <div class="charts-section">
      <div class="chart-card full-width">
        <div class="card-header">
          <h2>Динамика дефектов</h2>
          <span class="chart-period">Последние 7 дней</span>
        </div>
        <div class="chart-content">
          <LineChart
            :data="defectsTrendData"
            :width="chartWidth"
            :height="300"
            color="#667eea"
          />
        </div>
      </div>
      
      <div class="chart-card">
        <div class="card-header">
          <h2>Дефекты по статусам</h2>
        </div>
        <div class="chart-content">
          <PieChart
            :data="defectsByStatusData"
            :size="220"
          />
        </div>
      </div>
      
      <div class="chart-card">
        <div class="card-header">
          <h2>Дефекты по приоритетам</h2>
        </div>
        <div class="chart-content">
          <BarChart
            :data="defectsByPriorityData"
            :width="chartWidth / 2 - 20"
            :height="300"
          />
        </div>
      </div>
      
      <div class="chart-card full-width">
        <div class="card-header">
          <h2>Дефекты по проектам</h2>
        </div>
        <div class="chart-content">
          <BarChart
            :data="defectsByProjectData"
            :width="chartWidth"
            :height="300"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import PieChart from '../components/PieChart.vue';
import BarChart from '../components/BarChart.vue';
import LineChart from '../components/LineChart.vue';

const chartWidth = ref(800);

const reports = ref([
  {
    id: 1,
    title: 'Отчёт по проектам',
    description: 'Сводка по всем активным и завершённым проектам',
    icon: '📂',
    color: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)'
  },
  {
    id: 2,
    title: 'Отчёт по дефектам',
    description: 'Анализ дефектов по приоритетам и статусам',
    icon: '🐛',
    color: 'linear-gradient(135deg, #f59e0b 0%, #d97706 100%)'
  },
  {
    id: 3,
    title: 'Производительность команды',
    description: 'Метрики продуктивности и выполнения задач',
    icon: '👥',
    color: 'linear-gradient(135deg, #10b981 0%, #059669 100%)'
  },
  {
    id: 4,
    title: 'Временная аналитика',
    description: 'Анализ затрат времени на проекты и задачи',
    icon: '⏱️',
    color: 'linear-gradient(135deg, #3b82f6 0%, #2563eb 100%)'
  }
]);

const defectsByStatusData = ref([
  { label: 'Открыт', value: 12, color: '#ef4444' },
  { label: 'В работе', value: 18, color: '#3b82f6' },
  { label: 'На проверке', value: 8, color: '#f59e0b' },
  { label: 'Закрыт', value: 25, color: '#10b981' }
]);

const defectsByPriorityData = ref([
  { label: 'Высокий', value: 15, color: '#ef4444' },
  { label: 'Средний', value: 28, color: '#f59e0b' },
  { label: 'Низкий', value: 20, color: '#3b82f6' }
]);

const defectsByProjectData = ref([
  { label: 'CRM', value: 22, color: '#667eea' },
  { label: 'Веб-портал', value: 18, color: '#f59e0b' },
  { label: 'Мобильное', value: 12, color: '#10b981' },
  { label: 'Аналитика', value: 11, color: '#3b82f6' }
]);

const defectsTrendData = ref([
  { label: 'Пн', value: 8 },
  { label: 'Вт', value: 12 },
  { label: 'Ср', value: 10 },
  { label: 'Чт', value: 15 },
  { label: 'Пт', value: 18 },
  { label: 'Сб', value: 14 },
  { label: 'Вс', value: 11 }
]);

const viewReport = (report) => {
  console.log('[v0] Просмотр отчёта:', report);
};

const updateChartWidth = () => {
  const container = document.querySelector('.charts-section');
  if (container) {
    chartWidth.value = container.offsetWidth - 80;
  }
};

onMounted(() => {
  updateChartWidth();
  window.addEventListener('resize', updateChartWidth);
});

onUnmounted(() => {
  window.removeEventListener('resize', updateChartWidth);
});
</script>

<style scoped>
.reports-page {
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

.generate-button {
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

.generate-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 25px rgba(0, 0, 0, 0.2);
}

.button-icon {
  font-size: 1.25rem;
  line-height: 1;
}

.reports-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.report-card {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  border-radius: 16px;
  padding: 1.5rem;
  display: flex;
  gap: 1.5rem;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
}

.report-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.15);
}

.report-icon {
  width: 60px;
  height: 60px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.75rem;
  flex-shrink: 0;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.report-content {
  flex: 1;
}

.report-content h3 {
  font-size: 1.1rem;
  font-weight: 600;
  margin: 0 0 0.5rem;
  color: #2d3748;
}

.report-content p {
  color: #718096;
  font-size: 0.9rem;
  margin: 0 0 1rem;
  line-height: 1.5;
}

.view-button {
  background: none;
  border: none;
  color: #667eea;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  padding: 0;
  font-size: 0.95rem;
}

.view-button:hover {
  transform: translateX(5px);
}

.charts-section {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: 1.5rem;
}

.chart-card {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
}

.card-header {
  padding: 1.5rem;
  background: #f7fafc;
  border-bottom: 2px solid #e2e8f0;
}

.card-header h2 {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
  color: #2d3748;
}

.chart-placeholder {
  padding: 3rem 2rem;
  min-height: 300px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.chart-info {
  text-align: center;
}

.chart-icon {
  font-size: 3rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

.chart-info p {
  color: #718096;
  margin: 0.5rem 0;
}

.chart-hint {
  font-size: 0.85rem !important;
  color: #a0aec0 !important;
}

@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    gap: 1rem;
  }
  
  .reports-grid {
    grid-template-columns: 1fr;
  }
  
  .report-card {
    flex-direction: column;
  }
  
  .charts-section {
    grid-template-columns: 1fr;
  }
}
</style>
