<template>
  <div class="assignee-selector">
    <div class="current-assignee" @click="toggleDropdown">
      <div class="assignee-info">
        <div class="assignee-avatar" :style="{ background: getAvatarColor(currentAssignee) }">
          {{ getInitials(currentAssignee) }}
        </div>
        <div class="assignee-details">
          <span class="assignee-label">Исполнитель</span>
          <span class="assignee-name">{{ currentAssignee?.name || 'Не назначен' }}</span>
        </div>
      </div>
      <button 
        v-if="canAssign" 
        class="change-button"
        type="button"
      >
        <span>{{ currentAssignee ? '↻' : '+' }}</span>
      </button>
    </div>

    <Transition name="dropdown">
      <div v-if="showDropdown && canAssign" class="dropdown-menu">
        <div class="dropdown-header">
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Поиск сотрудника..."
            class="search-input"
            @click.stop
          />
        </div>
        
        <div class="users-list">
          <div
            v-if="currentAssignee"
            class="user-item unassign"
            @click="handleAssign(null)"
          >
            <div class="user-avatar">
              <span>×</span>
            </div>
            <div class="user-info">
              <span class="user-name">Снять назначение</span>
            </div>
          </div>

          <div
            v-for="user in filteredUsers"
            :key="user.id"
            :class="['user-item', { active: currentAssignee?.id === user.id }]"
            @click="handleAssign(user)"
          >
            <div class="user-avatar" :style="{ background: getAvatarColor(user) }">
              {{ getInitials(user) }}
            </div>
            <div class="user-info">
              <span class="user-name">{{ user.name }}</span>
              <span class="user-role">{{ user.role || 'Инженер' }}</span>
            </div>
            <span v-if="currentAssignee?.id === user.id" class="check-icon">✓</span>
          </div>

          <div v-if="filteredUsers.length === 0" class="empty-state">
            <span>Сотрудники не найдены</span>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { useAuthStore } from '../store/auth';

const auth = useAuthStore();
const showDropdown = ref(false);
const searchQuery = ref('');

const props = defineProps({
  assignee: {
    type: Object,
    default: null
  },
  users: {
    type: Array,
    required: true
  }
});

const emit = defineEmits(['assign']);

const canAssign = computed(() => true);

const currentAssignee = computed(() => {
  if (!props.assignee) return null;
  if (typeof props.assignee === 'object') return props.assignee;
  return props.users.find(u => u.id === props.assignee || u.name === props.assignee);
});

const filteredUsers = computed(() => {
  if (!searchQuery.value) return props.users;
  
  const query = searchQuery.value.toLowerCase();
  return props.users.filter(user => 
    user.name.toLowerCase().includes(query) ||
    (user.role && user.role.toLowerCase().includes(query))
  );
});

const toggleDropdown = () => {
  if (!canAssign.value) return;
  showDropdown.value = !showDropdown.value;
};

const handleAssign = (user) => {
  emit('assign', user);
  showDropdown.value = false;
  searchQuery.value = '';
};

const getInitials = (user) => {
  if (!user || !user.name) return '?';
  const names = user.name.split(' ');
  if (names.length >= 2) {
    return (names[0][0] + names[1][0]).toUpperCase();
  }
  return user.name.substring(0, 2).toUpperCase();
};

const getAvatarColor = (user) => {
  if (!user) return '#cbd5e0';
  
  const colors = [
    'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
    'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
    'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)',
    'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)',
    'linear-gradient(135deg, #fa709a 0%, #fee140 100%)',
    'linear-gradient(135deg, #30cfd0 0%, #330867 100%)',
    'linear-gradient(135deg, #a8edea 0%, #fed6e3 100%)',
    'linear-gradient(135deg, #ff9a9e 0%, #fecfef 100%)'
  ];
  
  const hash = user.name.split('').reduce((acc, char) => acc + char.charCodeAt(0), 0);
  return colors[hash % colors.length];
};

const handleClickOutside = (event) => {
  const selector = event.target.closest('.assignee-selector');
  if (!selector) {
    showDropdown.value = false;
  }
};

onMounted(() => {
  document.addEventListener('click', handleClickOutside);
});

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside);
});
</script>

<style scoped>
.assignee-selector {
  position: relative;
}

.current-assignee {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem;
  background: #f7fafc;
  border: 2px solid #e2e8f0;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.current-assignee:hover {
  background: white;
  border-color: #667eea;
  box-shadow: 0 0 0 4px rgba(102, 126, 234, 0.1);
}

.assignee-info {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex: 1;
}

.assignee-avatar {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 700;
  font-size: 1.1rem;
  flex-shrink: 0;
}

.assignee-details {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.assignee-label {
  font-size: 0.85rem;
  color: #718096;
  font-weight: 500;
}

.assignee-name {
  font-size: 1rem;
  color: #2d3748;
  font-weight: 600;
}

.change-button {
  width: 36px;
  height: 36px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border: none;
  border-radius: 8px;
  color: white;
  font-size: 1.25rem;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.change-button:hover {
  transform: scale(1.1);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}

.dropdown-menu {
  position: absolute;
  top: calc(100% + 0.5rem);
  left: 0;
  right: 0;
  background: white;
  border-radius: 12px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.15);
  z-index: 100;
  overflow: hidden;
}

.dropdown-header {
  padding: 1rem;
  border-bottom: 1px solid #e2e8f0;
}

.search-input {
  width: 100%;
  padding: 0.75rem 1rem;
  background: #f7fafc;
  border: 2px solid #e2e8f0;
  border-radius: 8px;
  color: #2d3748;
  font-size: 0.95rem;
  outline: none;
  transition: all 0.3s ease;
}

.search-input:focus {
  background: white;
  border-color: #667eea;
  box-shadow: 0 0 0 4px rgba(102, 126, 234, 0.1);
}

.users-list {
  max-height: 300px;
  overflow-y: auto;
}

.user-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 0.875rem 1rem;
  cursor: pointer;
  transition: all 0.2s ease;
  position: relative;
}

.user-item:hover {
  background: #f7fafc;
}

.user-item.active {
  background: rgba(102, 126, 234, 0.1);
}

.user-item.unassign {
  border-bottom: 1px solid #e2e8f0;
}

.user-item.unassign .user-avatar {
  background: #ef4444;
}

.user-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 700;
  font-size: 1rem;
  flex-shrink: 0;
}

.user-info {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  flex: 1;
}

.user-name {
  font-size: 0.95rem;
  color: #2d3748;
  font-weight: 600;
}

.user-role {
  font-size: 0.85rem;
  color: #718096;
}

.check-icon {
  color: #10b981;
  font-size: 1.25rem;
  font-weight: 700;
}

.empty-state {
  padding: 2rem 1rem;
  text-align: center;
  color: #718096;
  font-size: 0.95rem;
}

.dropdown-enter-active,
.dropdown-leave-active {
  transition: all 0.3s ease;
}

.dropdown-enter-from {
  opacity: 0;
  transform: translateY(-10px);
}

.dropdown-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}

@media (max-width: 768px) {
  .dropdown-menu {
    position: fixed;
    top: auto;
    bottom: 0;
    left: 0;
    right: 0;
    border-radius: 20px 20px 0 0;
    max-height: 70vh;
  }
  
  .users-list {
    max-height: calc(70vh - 80px);
  }
}
</style>
