<template>
  <Transition name="modal">
    <div v-if="isOpen" class="modal-overlay" @click.self="closeModal">
      <div class="modal-container">
        <div class="modal-header">
          <div class="header-icon">
            <span>🐛</span>
          </div>
          <div class="header-text">
            <h2>Добавить дефект</h2>
            <p>Заполните информацию о новом дефекте</p>
          </div>
          <button class="close-button" @click="closeModal">
            <span>×</span>
          </button>
        </div>

        <form @submit.prevent="handleSubmit" class="modal-form">
          <div class="form-row">
            <div class="form-group full-width">
              <label for="title">
                <span class="label-icon">📝</span>
                <span>Название дефекта</span>
                <span class="required">*</span>
              </label>
              <input
                id="title"
                v-model="formData.title"
                type="text"
                placeholder="Краткое описание проблемы"
                required
                class="form-input"
              />
            </div>
          </div>

          <div class="form-row">
            <div class="form-group full-width">
              <label for="description">
                <span class="label-icon">📄</span>
                <span>Подробное описание</span>
                <span class="required">*</span>
              </label>
              <textarea
                id="description"
                v-model="formData.description"
                placeholder="Опишите дефект подробно..."
                required
                rows="4"
                class="form-textarea"
              ></textarea>
            </div>
          </div>

          <div class="form-row two-columns">
            <div class="form-group">
              <label for="project">
                <span class="label-icon">📂</span>
                <span>Проект</span>
                <span class="required">*</span>
              </label>
              <select
                id="project"
                v-model="formData.projectId"
                required
                class="form-select"
              >
                <option value="" disabled>Выберите проект</option>
                <option
                  v-for="project in projects"
                  :key="project.id"
                  :value="project.id"
                >
                  {{ project.name }}
                </option>
              </select>
            </div>

            <div class="form-group">
              <label for="priority">
                <span class="label-icon">⚡</span>
                <span>Приоритет</span>
                <span class="required">*</span>
              </label>
              <select
                id="priority"
                v-model="formData.priority"
                required
                class="form-select"
              >
                <option value="" disabled>Выберите приоритет</option>
                <option value="high">Высокий</option>
                <option value="medium">Средний</option>
                <option value="low">Низкий</option>
              </select>
            </div>
          </div>

          <div class="form-row two-columns">
            <div class="form-group">
              <label for="deadline">
                <span class="label-icon">📅</span>
                <span>Дедлайн</span>
                <span class="required">*</span>
              </label>
              <input
                id="deadline"
                v-model="formData.deadline"
                type="date"
                required
                class="form-input"
              />
            </div>

            <div class="form-group" v-if="canAssign">
              <label for="assignee">
                <span class="label-icon">👤</span>
                <span>Исполнитель</span>
              </label>
              <select
                id="assignee"
                v-model="formData.assigneeId"
                class="form-select"
              >
                <option value="">Не назначен</option>
                <option
                  v-for="user in users"
                  :key="user.id"
                  :value="user.id"
                >
                  {{ user.username }}
                </option>
              </select>
            </div>
          </div>

          <div class="form-row">
            <div class="form-group full-width">
              <label>
                <span class="label-icon">📷</span>
                <span>Фотографии</span>
              </label>
              <div class="file-upload-area" @click="triggerFileInput">
                <input
                  ref="fileInput"
                  type="file"
                  accept="image/*"
                  multiple
                  @change="handleFileUpload"
                  class="file-input-hidden"
                />
                <div v-if="formData.photos.length === 0" class="upload-placeholder">
                  <span class="upload-icon">📁</span>
                  <p>Нажмите для загрузки фото</p>
                  <span class="upload-hint">PNG, JPG до 5MB</span>
                </div>
                <div v-else class="photos-preview">
                  <div
                    v-for="(photo, index) in formData.photos"
                    :key="index"
                    class="photo-item"
                  >
                    <img :src="photo.preview" :alt="`Photo ${index + 1}`" />
                    <button
                      type="button"
                      class="remove-photo"
                      @click.stop="removePhoto(index)"
                    >
                      ×
                    </button>
                  </div>
                  <div class="add-more" @click.stop="triggerFileInput">
                    <span>+</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="form-actions">
            <button type="button" class="cancel-button" @click="closeModal">
              Отмена
            </button>
            <button type="submit" class="submit-button" :disabled="isSubmitting">
              <span v-if="!isSubmitting" class="button-content">
                <span>Создать дефект</span>
                <span class="button-icon">✓</span>
              </span>
              <span v-else class="loading-content">
                <span class="loading-spinner"></span>
                <span>Создание...</span>
              </span>
            </button>
          </div>
        </form>
      </div>
    </div>
  </Transition>
</template>

<script setup>
import { ref, computed } from 'vue';
import { useAuthStore } from '../store/auth';

const props = defineProps({
  isOpen: {
    type: Boolean,
    required: true
  },
  projects: {
    type: Array,
    default: () => []
  },
  users: {
    type: Array,
    default: () => []
  }
});

const emit = defineEmits(['close', 'submit']);

const auth = useAuthStore();
const fileInput = ref(null);
const isSubmitting = ref(false);

const canAssign = computed(() => true);

const formData = ref({
  title: '',
  description: '',
  projectId: '',
  priority: '',
  deadline: '',
  assigneeId: '',
  photos: []
});

const closeModal = () => {
  emit('close');
  resetForm();
};

const resetForm = () => {
  formData.value = {
    title: '',
    description: '',
    projectId: '',
    priority: '',
    deadline: '',
    assigneeId: '',
    photos: []
  };
};

const triggerFileInput = () => {
  fileInput.value?.click();
};

const handleFileUpload = (event) => {
  const files = Array.from(event.target.files);
  files.forEach(file => {
    if (file.size > 5 * 1024 * 1024) {
      alert(`Файл ${file.name} слишком большой. Максимум 5MB.`);
      return;
    }
    
    const reader = new FileReader();
    reader.onload = (e) => {
      formData.value.photos.push({
        file,
        preview: e.target.result
      });
    };
    reader.readAsDataURL(file);
  });
  
  event.target.value = '';
};

const removePhoto = (index) => {
  formData.value.photos.splice(index, 1);
};

const handleSubmit = async () => {
  isSubmitting.value = true;
  
  try {
    const dateOnly = formData.value.deadline;
    
    const finalDataToSend = {
      title: formData.value.title,
      description: formData.value.description,
      projectId: formData.value.projectId, 
      assignedToId: formData.value.assigneeId || null, 
      priority: formData.value.priority,
      
      dueDate: dateOnly 
        ? new Date(dateOnly).toISOString() 
        : null,
      
    };
    const submitPayload = {
        data: finalDataToSend,
        photos: formData.value.photos.map(p => p.file) 
    }

    emit('submit', submitPayload);
    closeModal();
  } catch (error) {
    // ...
  } finally {
    isSubmitting.value = false;
  }
};
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.6);
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 1rem;
  overflow-y: auto;
}

.modal-container {
  background: rgba(255, 255, 255, 0.98);
  backdrop-filter: blur(20px);
  border-radius: 24px;
  width: 100%;
  max-width: 700px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 25px 60px rgba(0, 0, 0, 0.3);
  animation: slideUp 0.4s ease-out;
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(30px) scale(0.95);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

.modal-header {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 2rem 2rem 1.5rem;
  border-bottom: 1px solid #e2e8f0;
}

.header-icon {
  width: 60px;
  height: 60px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 2rem;
  flex-shrink: 0;
  box-shadow: 0 8px 20px rgba(102, 126, 234, 0.3);
}

.header-text {
  flex: 1;
}

.header-text h2 {
  font-size: 1.75rem;
  font-weight: 700;
  color: #2d3748;
  margin: 0 0 0.25rem;
}

.header-text p {
  color: #718096;
  font-size: 0.95rem;
  margin: 0;
}

.close-button {
  width: 40px;
  height: 40px;
  background: #f7fafc;
  border: none;
  border-radius: 10px;
  font-size: 2rem;
  line-height: 1;
  color: #718096;
  cursor: pointer;
  transition: all 0.2s;
  flex-shrink: 0;
}

.close-button:hover {
  background: #edf2f7;
  color: #2d3748;
  transform: rotate(90deg);
}

.modal-form {
  padding: 2rem;
}

.form-row {
  display: flex;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.form-row.two-columns {
  display: grid;
  grid-template-columns: 1fr 1fr;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-group.full-width {
  width: 100%;
}

.form-group label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #2d3748;
  font-weight: 600;
  font-size: 0.95rem;
}

.label-icon {
  font-size: 1.1rem;
}

.required {
  color: #ef4444;
  font-weight: 700;
}

.form-input,
.form-select,
.form-textarea {
  width: 100%;
  padding: 0.875rem 1rem;
  background: #f7fafc;
  border: 2px solid #e2e8f0;
  border-radius: 12px;
  color: #2d3748;
  font-size: 0.95rem;
  outline: none;
  transition: all 0.3s ease;
  font-family: inherit;
}

.form-input:focus,
.form-select:focus,
.form-textarea:focus {
  background: white;
  border-color: #667eea;
  box-shadow: 0 0 0 4px rgba(102, 126, 234, 0.1);
}

.form-textarea {
  resize: vertical;
  min-height: 100px;
}

.form-select {
  appearance: none;
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' viewBox='0 0 24 24' fill='none' stroke='%23718096' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Cpath d='M6 9l6 6 6-6'/%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 1rem center;
  background-size: 16px;
  padding-right: 3rem;
}

.file-upload-area {
  background: #f7fafc;
  border: 2px dashed #cbd5e0;
  border-radius: 12px;
  padding: 2rem;
  cursor: pointer;
  transition: all 0.3s ease;
}

.file-upload-area:hover {
  border-color: #667eea;
  background: #edf2f7;
}

.file-input-hidden {
  display: none;
}

.upload-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
  text-align: center;
}

.upload-icon {
  font-size: 3rem;
}

.upload-placeholder p {
  color: #2d3748;
  font-weight: 600;
  margin: 0;
}

.upload-hint {
  color: #718096;
  font-size: 0.875rem;
}

.photos-preview {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(100px, 1fr));
  gap: 1rem;
}

.photo-item {
  position: relative;
  aspect-ratio: 1;
  border-radius: 12px;
  overflow: hidden;
  background: #edf2f7;
}

.photo-item img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.remove-photo {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
  width: 28px;
  height: 28px;
  background: rgba(239, 68, 68, 0.95);
  border: none;
  border-radius: 50%;
  color: white;
  font-size: 1.5rem;
  line-height: 1;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
}

.remove-photo:hover {
  background: #dc2626;
  transform: scale(1.1);
}

.add-more {
  aspect-ratio: 1;
  border: 2px dashed #cbd5e0;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 2rem;
  color: #718096;
  cursor: pointer;
  transition: all 0.3s;
}

.add-more:hover {
  border-color: #667eea;
  color: #667eea;
  background: rgba(102, 126, 234, 0.05);
}

.form-actions {
  display: flex;
  gap: 1rem;
  margin-top: 2rem;
  padding-top: 1.5rem;
  border-top: 1px solid #e2e8f0;
}

.cancel-button,
.submit-button {
  flex: 1;
  padding: 1rem 1.5rem;
  border: none;
  border-radius: 12px;
  font-weight: 700;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.3s ease;
}

.cancel-button {
  background: #f7fafc;
  color: #718096;
}

.cancel-button:hover {
  background: #edf2f7;
  color: #2d3748;
}

.submit-button {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  box-shadow: 0 10px 25px rgba(102, 126, 234, 0.3);
}

.submit-button:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 15px 35px rgba(102, 126, 234, 0.4);
}

.submit-button:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.button-content,
.loading-content {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.button-icon {
  font-size: 1.25rem;
}

.loading-spinner {
  width: 18px;
  height: 18px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top: 2px solid white;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

@media (max-width: 768px) {
  .modal-container {
    max-height: 95vh;
  }
  
  .modal-header {
    padding: 1.5rem;
  }
  
  .header-icon {
    width: 50px;
    height: 50px;
    font-size: 1.75rem;
  }
  
  .header-text h2 {
    font-size: 1.5rem;
  }
  
  .modal-form {
    padding: 1.5rem;
  }
  
  .form-row.two-columns {
    grid-template-columns: 1fr;
  }
  
  .form-actions {
    flex-direction: column;
  }
}
</style>
