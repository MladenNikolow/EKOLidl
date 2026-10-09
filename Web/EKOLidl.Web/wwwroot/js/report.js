// Report form: inline validation in Bulgarian, character counter, photo previews and drag & drop.
// Without this file the browser's native validation and the server-side checks still apply.
(() => {
  'use strict';

  const form = document.querySelector('[data-report-form]');
  if (!form) return;

  form.noValidate = true;
  let submitted = false;
  const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  const fileInput = form.querySelector('input[type="file"]');
  const maxFiles = Number(fileInput.dataset.maxFiles);
  const maxBytes = Number(fileInput.dataset.maxBytes);
  const allowedTypes = ['image/jpeg', 'image/png', 'image/webp'];
  const group = form.querySelector('[data-choice-group]');
  const otherBox = form.querySelector('[data-other]');
  const otherInput = form.querySelector('[data-required-when-other]');
  const rules = form.querySelector('input[type="checkbox"][name="CheckedRules"]');
  const fields = [...form.querySelectorAll('input:not([type="hidden"]):not([type="checkbox"]), textarea'), rules];

  // Server-rendered errors: mark their fields as invalid for assistive technology.
  form.querySelectorAll('.input-validation-error').forEach((field) => field.setAttribute('aria-invalid', 'true'));

  const setError = (name, field, message) => {
    const target = form.querySelector(`[data-valmsg-for="${name}"]`);
    if (target) {
      target.textContent = message;
      target.classList.toggle('field-validation-error', Boolean(message));
      target.classList.toggle('field-validation-valid', !message);
    }
    if (field) {
      if (message) field.setAttribute('aria-invalid', 'true');
      else field.removeAttribute('aria-invalid');
    }
  };

  const fileMessage = () => {
    const files = [...fileInput.files];
    if (!files.length) return fileInput.dataset.msgRequired;
    if (files.length > maxFiles) return fileInput.dataset.msgCount;
    if (files.some((file) => !allowedTypes.includes(file.type) || file.size > maxBytes)) return fileInput.dataset.msgFile;
    return '';
  };

  const messageFor = (field) => {
    if (field === fileInput) return fileMessage();
    if (field === otherInput) return otherBox?.checked && !field.value.trim() ? field.dataset.msgRequired : '';

    const value = field.value.trim();
    const { validity } = field;
    if (validity.valueMissing || (field.required && field.type !== 'checkbox' && !value)) return field.dataset.msgRequired;
    // tooShort only fires after the user edits a field, so check lengths directly as well.
    if (validity.tooShort || validity.tooLong || (field.minLength > 0 && value.length < field.minLength)) return field.dataset.msgLength;
    if (validity.patternMismatch) return field.dataset.msgPattern;
    if (validity.typeMismatch) return field.dataset.msgType;
    return validity.valid ? '' : field.validationMessage;
  };

  const validateField = (field) => {
    const message = messageFor(field) || '';
    setError(field.name, field, message);
    return !message;
  };

  const validateGroup = () => {
    const valid = Boolean(group.querySelector('input:checked'));
    setError('TrashType', null, valid ? '' : group.dataset.msgRequired);
    return valid;
  };

  form.addEventListener('submit', (event) => {
    submitted = true;
    let first = null;
    fields.forEach((field) => {
      if (!validateField(field) && !first) first = field;
    });
    if (!validateGroup() && !first) first = group.querySelector('input');

    if (first) {
      event.preventDefault();
      first.closest('.field, fieldset')?.scrollIntoView({ block: 'center', behavior: reduceMotion ? 'auto' : 'smooth' });
      first.focus({ preventScroll: true });
      return;
    }

    const button = form.querySelector('[data-submit]');
    button.disabled = true;
    button.querySelector('[data-submit-label]').textContent = 'Изпращане…';
  });

  fields.forEach((field) => {
    field.addEventListener('blur', () => {
      if (submitted || field.value) validateField(field);
    });
    field.addEventListener('input', () => {
      if (submitted || field.hasAttribute('aria-invalid')) validateField(field);
    });
  });

  group.addEventListener('change', () => {
    if (!submitted) return;
    validateGroup();
    if (otherInput) validateField(otherInput);
  });

  // Character counter
  const description = form.querySelector('#Description');
  const counter = form.querySelector('[data-counter-for="Description"]');
  if (description && counter) {
    const update = () => { counter.textContent = `${description.value.length}/${description.maxLength}`; };
    description.addEventListener('input', update);
    update();
  }

  // Photo previews
  const previews = form.querySelector('[data-previews]');
  const renderPreviews = () => {
    previews.querySelectorAll('img').forEach((img) => URL.revokeObjectURL(img.src));
    previews.replaceChildren(...[...fileInput.files].map((file) => {
      const item = document.createElement('li');
      item.className = 'media aspect-square overflow-hidden rounded-lg';
      if (allowedTypes.includes(file.type)) {
        const img = document.createElement('img');
        img.src = URL.createObjectURL(file);
        img.alt = file.name;
        item.append(img);
      } else {
        item.className += ' grid place-items-center p-2 text-center text-xs text-white';
        item.textContent = file.name;
      }
      return item;
    }));
  };
  fileInput.addEventListener('change', () => {
    renderPreviews();
    validateField(fileInput);
  });

  // Drag & drop onto the drop zone
  const zone = form.querySelector('[data-dropzone]');
  ['dragenter', 'dragover'].forEach((type) => zone.addEventListener(type, (event) => {
    event.preventDefault();
    zone.setAttribute('data-dragging', '');
  }));
  ['dragleave', 'drop'].forEach((type) => zone.addEventListener(type, (event) => {
    event.preventDefault();
    zone.removeAttribute('data-dragging');
  }));
  zone.addEventListener('drop', (event) => {
    if (!event.dataTransfer?.files?.length) return;
    fileInput.files = event.dataTransfer.files;
    fileInput.dispatchEvent(new Event('change'));
  });

  // After a failed server-side check, move focus to the error summary.
  form.querySelector('[data-error-summary]')?.focus();
})();
