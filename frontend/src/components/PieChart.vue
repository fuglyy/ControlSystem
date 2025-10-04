<template>
  <div class="pie-chart">
    <svg :width="size" :height="size" :viewBox="`0 0 ${size} ${size}`">
      <g :transform="`translate(${size / 2}, ${size / 2})`">
        <path
          v-for="(segment, index) in segments"
          :key="index"
          :d="segment.path"
          :fill="segment.color"
          :class="['pie-segment', { active: hoveredIndex === index }]"
          @mouseenter="hoveredIndex = index"
          @mouseleave="hoveredIndex = null"
        >
          <title>{{ segment.label }}: {{ segment.value }} ({{ segment.percentage }}%)</title>
        </path>
      </g>
    </svg>
    
    <div class="legend">
      <div
        v-for="(item, index) in data"
        :key="index"
        :class="['legend-item', { active: hoveredIndex === index }]"
        @mouseenter="hoveredIndex = index"
        @mouseleave="hoveredIndex = null"
      >
        <div class="legend-color" :style="{ background: item.color }"></div>
        <div class="legend-content">
          <span class="legend-label">{{ item.label }}</span>
          <span class="legend-value">{{ item.value }} ({{ getPercentage(item.value) }}%)</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';

const props = defineProps({
  data: {
    type: Array,
    required: true
  },
  size: {
    type: Number,
    default: 200
  }
});

const hoveredIndex = ref(null);

const total = computed(() => {
  return props.data.reduce((sum, item) => sum + item.value, 0);
});

const getPercentage = (value) => {
  return ((value / total.value) * 100).toFixed(1);
};

const segments = computed(() => {
  let currentAngle = -90;
  const radius = props.size / 2 - 10;
  
  return props.data.map(item => {
    const percentage = (item.value / total.value) * 100;
    const angle = (percentage / 100) * 360;
    
    const startAngle = currentAngle;
    const endAngle = currentAngle + angle;
    
    const startRad = (startAngle * Math.PI) / 180;
    const endRad = (endAngle * Math.PI) / 180;
    
    const x1 = radius * Math.cos(startRad);
    const y1 = radius * Math.sin(startRad);
    const x2 = radius * Math.cos(endRad);
    const y2 = radius * Math.sin(endRad);
    
    const largeArc = angle > 180 ? 1 : 0;
    
    const path = [
      `M 0 0`,
      `L ${x1} ${y1}`,
      `A ${radius} ${radius} 0 ${largeArc} 1 ${x2} ${y2}`,
      `Z`
    ].join(' ');
    
    currentAngle = endAngle;
    
    return {
      path,
      color: item.color,
      label: item.label,
      value: item.value,
      percentage: percentage.toFixed(1)
    };
  });
});
</script>

<style scoped>
.pie-chart {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 2rem;
}

.pie-segment {
  cursor: pointer;
  transition: all 0.3s ease;
  opacity: 0.9;
}

.pie-segment:hover,
.pie-segment.active {
  opacity: 1;
  filter: brightness(1.1);
  transform: scale(1.05);
  transform-origin: center;
}

.legend {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  width: 100%;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem;
  border-radius: 8px;
  transition: all 0.2s ease;
  cursor: pointer;
}

.legend-item:hover,
.legend-item.active {
  background: #f7fafc;
}

.legend-color {
  width: 16px;
  height: 16px;
  border-radius: 4px;
  flex-shrink: 0;
}

.legend-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex: 1;
  gap: 1rem;
}

.legend-label {
  font-weight: 600;
  color: #2d3748;
  font-size: 0.95rem;
}

.legend-value {
  color: #718096;
  font-size: 0.9rem;
  font-weight: 500;
}
</style>
