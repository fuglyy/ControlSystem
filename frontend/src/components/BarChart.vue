<template>
  <div class="bar-chart">
    <svg :width="width" :height="height" :viewBox="`0 0 ${width} ${height}`">
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
        
         Bars 
        <g
          v-for="(item, index) in data"
          :key="index"
          :class="['bar-group', { active: hoveredIndex === index }]"
          @mouseenter="hoveredIndex = index"
          @mouseleave="hoveredIndex = null"
        >
          <rect
            :x="getXPosition(index)"
            :y="getYPosition(item.value)"
            :width="barWidth"
            :height="chartHeight - getYPosition(item.value)"
            :fill="item.color"
            rx="4"
            class="bar"
          >
            <title>{{ item.label }}: {{ item.value }}</title>
          </rect>
          
           Value label on top of bar 
          <text
            :x="getXPosition(index) + barWidth / 2"
            :y="getYPosition(item.value) - 8"
            text-anchor="middle"
            class="bar-value"
          >
            {{ item.value }}
          </text>
        </g>
        
         X-axis labels 
        <text
          v-for="(item, index) in data"
          :key="`label-${index}`"
          :x="getXPosition(index) + barWidth / 2"
          :y="chartHeight + 20"
          text-anchor="middle"
          class="axis-label"
        >
          {{ item.label }}
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

const barWidth = computed(() => {
  const totalGap = chartWidth.value * 0.2;
  const gapBetweenBars = totalGap / (props.data.length + 1);
  return (chartWidth.value - totalGap) / props.data.length;
});

const getXPosition = (index) => {
  const totalGap = chartWidth.value * 0.2;
  const gapBetweenBars = totalGap / (props.data.length + 1);
  return gapBetweenBars * (index + 1) + barWidth.value * index;
};

const getYPosition = (value) => {
  return chartHeight.value - (value / maxValue.value) * chartHeight.value;
};
</script>

<style scoped>
.bar-chart {
  width: 100%;
  overflow-x: auto;
}

.bar-group {
  cursor: pointer;
}

.bar {
  transition: all 0.3s ease;
  opacity: 0.9;
}

.bar-group:hover .bar,
.bar-group.active .bar {
  opacity: 1;
  filter: brightness(1.1);
}

.bar-value {
  font-size: 12px;
  font-weight: 600;
  fill: #2d3748;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.bar-group:hover .bar-value,
.bar-group.active .bar-value {
  opacity: 1;
}

.axis-label {
  font-size: 11px;
  fill: #718096;
  font-weight: 500;
}
</style>
