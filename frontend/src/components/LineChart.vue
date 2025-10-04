<template>
  <div class="line-chart">
    <svg :width="width" :height="height" :viewBox="`0 0 ${width} ${height}`">
      <defs>
        <linearGradient id="lineGradient" x1="0%" y1="0%" x2="0%" y2="100%">
          <stop offset="0%" :style="{ stopColor: color, stopOpacity: 0.3 }" />
          <stop offset="100%" :style="{ stopColor: color, stopOpacity: 0 }" />
        </linearGradient>
      </defs>
      
      <g :transform="`translate(${padding.left}, ${padding.top})`">
         Y-axis grid lines 
        <line
          v-for="tick in yTicks"
          :key="`grid-${tick}`"
          :x1="0"
          :y1="getYPosition(tick)"
          :x2="chartWidth"
          :y2="getYPosition(tick)"
          stroke="#e2e8f0"
          stroke-width="1"
        />
        
         Area under line 
        <path
          :d="areaPath"
          fill="url(#lineGradient)"
        />
        
         Line 
        <path
          :d="linePath"
          :stroke="color"
          stroke-width="3"
          fill="none"
          stroke-linecap="round"
          stroke-linejoin="round"
          class="line-path"
        />
        
         Data points 
        <g
          v-for="(point, index) in data"
          :key="index"
          :class="['point-group', { active: hoveredIndex === index }]"
          @mouseenter="hoveredIndex = index"
          @mouseleave="hoveredIndex = null"
        >
          <circle
            :cx="getXPosition(index)"
            :cy="getYPosition(point.value)"
            r="5"
            :fill="color"
            class="data-point"
          >
            <title>{{ point.label }}: {{ point.value }}</title>
          </circle>
          
           Value label 
          <text
            v-if="hoveredIndex === index"
            :x="getXPosition(index)"
            :y="getYPosition(point.value) - 15"
            text-anchor="middle"
            class="point-value"
          >
            {{ point.value }}
          </text>
        </g>
        
         X-axis labels 
        <text
          v-for="(point, index) in data"
          :key="`label-${index}`"
          :x="getXPosition(index)"
          :y="chartHeight + 20"
          text-anchor="middle"
          class="axis-label"
        >
          {{ point.label }}
        </text>
        
         Y-axis labels 
        <text
          v-for="tick in yTicks"
          :key="`y-label-${tick}`"
          :x="-10"
          :y="getYPosition(tick) + 4"
          text-anchor="end"
          class="axis-label"
        >
          {{ tick }}
        </text>
      </g>
    </svg>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';

const props = defineProps({
  data: {
    type: Array,
    required: true
  },
  width: {
    type: Number,
    default: 600
  },
  height: {
    type: Number,
    default: 300
  },
  color: {
    type: String,
    default: '#667eea'
  }
});

const hoveredIndex = ref(null);

const padding = {
  top: 20,
  right: 20,
  bottom: 40,
  left: 50
};

const chartWidth = computed(() => props.width - padding.left - padding.right);
const chartHeight = computed(() => props.height - padding.top - padding.bottom);

const maxValue = computed(() => {
  const max = Math.max(...props.data.map(d => d.value));
  return Math.ceil(max / 10) * 10;
});

const yTicks = computed(() => {
  const ticks = [];
  const step = maxValue.value / 5;
  for (let i = 0; i <= 5; i++) {
    ticks.push(Math.round(step * i));
  }
  return ticks;
});

const getXPosition = (index) => {
  return (chartWidth.value / (props.data.length - 1)) * index;
};

const getYPosition = (value) => {
  return chartHeight.value - (value / maxValue.value) * chartHeight.value;
};

const linePath = computed(() => {
  return props.data.map((point, index) => {
    const x = getXPosition(index);
    const y = getYPosition(point.value);
    return `${index === 0 ? 'M' : 'L'} ${x} ${y}`;
  }).join(' ');
});

const areaPath = computed(() => {
  const line = props.data.map((point, index) => {
    const x = getXPosition(index);
    const y = getYPosition(point.value);
    return `${index === 0 ? 'M' : 'L'} ${x} ${y}`;
  }).join(' ');
  
  const lastX = getXPosition(props.data.length - 1);
  return `${line} L ${lastX} ${chartHeight.value} L 0 ${chartHeight.value} Z`;
});
</script>

<style scoped>
.line-chart {
  width: 100%;
  overflow-x: auto;
}

.line-path {
  filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.1));
}

.point-group {
  cursor: pointer;
}

.data-point {
  transition: all 0.3s ease;
  filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.2));
}

.point-group:hover .data-point,
.point-group.active .data-point {
  r: 7;
  filter: drop-shadow(0 4px 8px rgba(0, 0, 0, 0.3));
}

.point-value {
  font-size: 12px;
  font-weight: 700;
  fill: #2d3748;
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(5px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.axis-label {
  font-size: 11px;
  fill: #718096;
  font-weight: 500;
}
</style>
