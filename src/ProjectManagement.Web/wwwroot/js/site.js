const headers = {
  "Content-Type": "application/json",
  "Accept": "application/json"
};

const authValidation = {
  namePattern: /^[A-Za-z][A-Za-z .'-]*$/,
  emailPattern: /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/,
  passwordPattern: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,100}$/
};

function findField(form, name) {
  return form.querySelector(`[name="${name}"]`);
}

function setFieldError(form, name, message) {
  const field = findField(form, name);
  const messageElement = form.querySelector(`[data-valmsg-for="${name}"]`);

  if (field)
    field.setAttribute("aria-invalid", message ? "true" : "false");

  if (messageElement) {
    messageElement.setAttribute("aria-live", "polite");
    messageElement.textContent = message;
  }
}

function getFieldValue(form, name) {
  return findField(form, name)?.value ?? "";
}

function validateEmailField(form) {
  const email = getFieldValue(form, "Input.Email").trim();

  if (!email) {
    setFieldError(form, "Input.Email", "Email address is required.");
    return false;
  }

  if (!authValidation.emailPattern.test(email)) {
    setFieldError(form, "Input.Email", "Enter a valid email address, for example name@example.com.");
    return false;
  }

  setFieldError(form, "Input.Email", "");
  return true;
}

function validatePasswordField(form, requireComplexPassword) {
  const password = getFieldValue(form, "Input.Password");

  if (!password) {
    setFieldError(form, "Input.Password", "Password is required.");
    return false;
  }

  if (requireComplexPassword && !authValidation.passwordPattern.test(password)) {
    setFieldError(form, "Input.Password", "Password must include uppercase, lowercase, number, and special character.");
    return false;
  }

  setFieldError(form, "Input.Password", "");
  return true;
}

function validateFullNameField(form) {
  const fullName = getFieldValue(form, "Input.FullName").trim();

  if (!fullName) {
    setFieldError(form, "Input.FullName", "Full name is required.");
    return false;
  }

  if (fullName.length < 2 || fullName.length > 100) {
    setFieldError(form, "Input.FullName", "Full name must be between 2 and 100 characters.");
    return false;
  }

  if (!authValidation.namePattern.test(fullName)) {
    setFieldError(form, "Input.FullName", "Full name can contain only letters, spaces, apostrophes, dots, and hyphens.");
    return false;
  }

  setFieldError(form, "Input.FullName", "");
  return true;
}

function validateConfirmPasswordField(form) {
  const password = getFieldValue(form, "Input.Password");
  const confirmPassword = getFieldValue(form, "Input.ConfirmPassword");

  if (!confirmPassword) {
    setFieldError(form, "Input.ConfirmPassword", "Confirm password is required.");
    return false;
  }

  if (password !== confirmPassword) {
    setFieldError(form, "Input.ConfirmPassword", "Password and confirm password must match.");
    return false;
  }

  setFieldError(form, "Input.ConfirmPassword", "");
  return true;
}

function validateLoginForm(form) {
  const checks = [
    validateEmailField(form),
    validatePasswordField(form, false)
  ];

  return checks.every(Boolean);
}

function validateRegisterForm(form) {
  const checks = [
    validateFullNameField(form),
    validateEmailField(form),
    validatePasswordField(form, true),
    validateConfirmPasswordField(form)
  ];

  return checks.every(Boolean);
}

function validateAuthField(form, fieldName) {
  const isRegisterForm = form.dataset.authForm === "register";

  switch (fieldName) {
    case "Input.FullName":
      return validateFullNameField(form);
    case "Input.Email":
      return validateEmailField(form);
    case "Input.Password":
      return validatePasswordField(form, isRegisterForm);
    case "Input.ConfirmPassword":
      return validateConfirmPasswordField(form);
    default:
      return true;
  }
}

function validateChangedAuthField(form, field) {
  if (!field.name)
    return;

  validateAuthField(form, field.name);

  if (form.dataset.authForm === "register" && field.name === "Input.Password") {
    const confirmPassword = getFieldValue(form, "Input.ConfirmPassword");

    if (confirmPassword)
      validateConfirmPasswordField(form);
  }
}

document.querySelectorAll("[data-auth-form]").forEach(form => {
  form.addEventListener("input", event => {
    validateChangedAuthField(form, event.target);
  });

// Admin edit handling (prefill simple modal / prompt)
const editModal = document.getElementById('editTaskModal');
const editForm = document.getElementById('editTaskForm');

async function openEditModal(taskId, rowVersion) {
  if (!editModal) return;

  editModal.style.display = 'block';
  editModal.removeAttribute('aria-hidden');

  editForm.querySelector('input[name="taskId"]').value = taskId;
  editForm.querySelector('input[name="rowVersion"]').value = rowVersion;

  // Load employees for select
  const select = editForm.querySelector('select[name="assignedEmployeeId"]');
  select.innerHTML = '';
  try {
    const employees = await sendJson('/api/employees', 'GET');
    employees.forEach(emp => {
      const opt = document.createElement('option');
      opt.value = emp.id;
      opt.textContent = `${emp.name} (${emp.email})`;
      select.appendChild(opt);
    });
  } catch {
    // ignore if cannot load employees
  }
}

document.querySelectorAll('[data-edit-task-id]').forEach(button => {
  button.addEventListener('click', event => {
    const id = button.dataset.editTaskId;
    const rowVersion = button.dataset.rowVersion || document.querySelector(`tr[data-task-id="${id}"]`)?.dataset.rowVersion;
    openEditModal(id, rowVersion);
  });
});

document.querySelectorAll('[data-close-modal]').forEach(btn => btn.addEventListener('click', () => {
  if (!editModal) return;
  editModal.style.display = 'none';
  editModal.setAttribute('aria-hidden', 'true');
}));

if (editForm) {
  editForm.addEventListener('submit', async event => {
    event.preventDefault();
    const fd = new FormData(editForm);
    const id = fd.get('taskId');
    const payload = {
      title: fd.get('title'),
      description: fd.get('description'),
      assignedEmployeeId: fd.get('assignedEmployeeId'),
      rowVersion: fd.get('rowVersion')
    };

    try {
      await sendJson(`/api/tasks/${id}`, 'PUT', payload);
      window.location.reload();
    } catch (err) {
      alert(err.message);
    }
  });
}

  form.addEventListener("change", event => {
    validateChangedAuthField(form, event.target);
  });

  form.addEventListener("submit", event => {
    const mode = form.dataset.authForm;
    const isValid = mode === "register"
      ? validateRegisterForm(form)
      : validateLoginForm(form);

    if (!isValid)
      event.preventDefault();
  });
});

async function sendJson(url, method, body) {
  const response = await fetch(url, {
    method,
    headers,
    credentials: "same-origin",
    body: body ? JSON.stringify(body) : undefined
  });

  if (!response.ok) {
    let message = "Request failed.";

    try {
      const payload = await response.json();
      message = getProblemDetailsMessage(payload, message);
    } catch {
      message = await response.text() || message;
    }

    throw new Error(message);
  }

  if (response.status === 204)
    return null;

  return response.json();
}

function getProblemDetailsMessage(payload, fallbackMessage) {
  if (payload?.errors) {
    const messages = Object.values(payload.errors)
      .flatMap(value => Array.isArray(value) ? value : [value])
      .filter(Boolean);

    if (messages.length > 0)
      return messages.join("\n");
  }

  return payload?.detail
    || payload?.error
    || payload?.title
    || fallbackMessage;
}

document.querySelector("[data-create-project]")?.addEventListener("submit", async event => {
  event.preventDefault();

  const form = event.currentTarget;
  const data = Object.fromEntries(new FormData(form));

  try {
    const project = await sendJson("/api/projects", "POST", {
      name: data.name,
      description: data.description
    });

    window.location.assign(`/Projects/Details/${project.id}`);
  } catch (error) {
    alert(error.message);
  }
});

document.querySelector("[data-create-task]")?.addEventListener("submit", async event => {
  event.preventDefault();

  const form = event.currentTarget;
  const data = Object.fromEntries(new FormData(form));
  const projectId = form.dataset.projectId;

  try {
    await sendJson(`/api/projects/${projectId}/tasks`, "POST", {
      title: data.title,
      description: data.description,
      assignedEmployeeId: data.assignedEmployeeId
    });

    window.location.reload();
  } catch (error) {
    alert(error.message);
  }
});

document.querySelectorAll("[data-task-status]").forEach(button => {
  button.addEventListener("click", async event => {
    const currentButton = event.currentTarget;

    try {
      currentButton.disabled = true;
      const rowVersion = currentButton.dataset.rowVersion || document.querySelector(`tr[data-task-id="${currentButton.dataset.taskStatus}"]`)?.dataset.rowVersion;

      await sendJson(`/api/tasks/${currentButton.dataset.taskStatus}/status`, "PATCH", {
        status: currentButton.dataset.status,
        rowVersion: rowVersion
      });

      window.location.reload();
    } catch (error) {
      currentButton.disabled = false;
      alert(error.message);
    }
  });
});
