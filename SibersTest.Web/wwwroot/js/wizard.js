const wizard = {
    currentStep: 1,
    totalSteps: 5,
    data: {
        id: 0, // Added ID for Edit mode
        name: '', startDate: '', endDate: '', priority: 0,
        customerCompany: '', executorCompany: '',
        managerId: null, managerName: '',
        employees: [], 
        files: [],
        existingFiles: []
    },

    init() {
        // Prefer data from server (Edit mode), otherwise load draft from localStorage
        if (typeof serverModel !== 'undefined' && serverModel && serverModel.Id > 0) {
            this.loadFromServer(serverModel);
        } else {
            this.loadFromLocalStorage();
        }
        
        this.bindEvents();
        this.updateUI();
    },

    bindEvents() {
        document.getElementById('next-btn').onclick = () => this.moveStep(1);
        document.getElementById('prev-btn').onclick = () => this.moveStep(-1);
        document.getElementById('submit-btn').onclick = () => this.submit();
        
        this.initManagerSearch();
        this.initEmployeeSearch();
        this.initFileHandlers();
    },

    // --- Navigation and Validation ---
    moveStep(direction) {


        // Debug: show current step data in console
        console.log("Current Step:", this.currentStep);
        console.log("Current Data:", this.data);

        if (direction === 1 && !this.validateCurrentStep()) return;
        
        this.saveCurrentStepData();
        
        document.getElementById(`step-${this.currentStep}`).classList.add('d-none');
        this.currentStep += direction;
        document.getElementById(`step-${this.currentStep}`).classList.remove('d-none');
        
        this.updateUI();
        localStorage.setItem('wizard_step', this.currentStep);
    },

    validateCurrentStep() {
        const currentForm = document.getElementById(`step-${this.currentStep}`);
        const inputs = currentForm.querySelectorAll('input[required]');
        
        // Basic required fields validation
        for (let input of inputs) {
            if (!input.value) {
                input.classList.add('is-invalid');
                alert("Please fill in all required fields!");
                return false;
            }
            input.classList.remove('is-invalid');
        }
        
        // Step 1: Validate dates (start must be before end)
        if (this.currentStep === 1) {
            const start = new Date(document.getElementById('StartDate').value);
            const end = new Date(document.getElementById('EndDate').value);
            if (document.getElementById('EndDate').value && start > end) {
                alert("Start date cannot be after end date!");
                document.getElementById('EndDate').classList.add('is-invalid');
                return false;
            }
        }
        
        if (this.currentStep === 3 && !this.data.managerId) {
            alert("Please select a manager!"); return false;
        }
        return true;
    },

    updateUI() {
        const percent = (this.currentStep / this.totalSteps) * 100;
        const progress = document.getElementById('wizard-progress');
        if(progress) progress.style.width = percent + '%';
        
        document.getElementById('step-count').innerText = `${this.currentStep} of ${this.totalSteps}`;
        const titles = ["Project Details", "Companies", "Manager", "Team", "Documents"];
        document.getElementById('step-title').innerText = `Step ${this.currentStep}: ${titles[this.currentStep-1]}`;

        document.getElementById('prev-btn').style.display = this.currentStep > 1 ? 'block' : 'none';
        document.getElementById('next-btn').style.display = this.currentStep < this.totalSteps ? 'block' : 'none';
        document.getElementById('submit-btn').style.display = this.currentStep === this.totalSteps ? 'block' : 'none';
    },

    // --- Data Handling ---
    saveCurrentStepData() {
        if (this.currentStep === 1) {
            this.data.name = document.getElementById('Name').value;
            this.data.startDate = document.getElementById('StartDate').value;
            this.data.endDate = document.getElementById('EndDate').value;
            this.data.priority = document.getElementById('Priority').value;
        } else if (this.currentStep === 2) {
            this.data.customerCompany = document.getElementById('CustomerCompany').value;
            this.data.executorCompany = document.getElementById('ExecutorCompany').value;
        }
        this.saveToLocalStorage();
    },

    loadFromServer(model) {
    console.log("Model from server:", model);
    localStorage.removeItem('wizard_backup'); 

    // Map main data fields from server model
    this.data.id = model.Id || model.id || 0;
    this.data.name = model.Name || model.name || '';
    this.data.startDate = (model.StartDate || model.startDate || '').split('T')[0];
    this.data.endDate = (model.EndDate || model.endDate || '').split('T')[0];
    this.data.priority = model.Priority || model.priority || 0;
    this.data.customerCompany = model.CustomerCompany || model.customerCompany || '';
    this.data.executorCompany = model.ExecutorCompany || model.executorCompany || '';

    // map employees list
    const employeesList = model.Employees || model.employees || [];
    this.data.employees = employeesList;

    // Fill visual fields (inputs)
    document.getElementById('Name').value = this.data.name;
    document.getElementById('StartDate').value = this.data.startDate;
    document.getElementById('EndDate').value = this.data.endDate;
    document.getElementById('Priority').value = this.data.priority;
    document.getElementById('CustomerCompany').value = this.data.customerCompany;
    document.getElementById('ExecutorCompany').value = this.data.executorCompany;

    // Render manager info
    const mId = model.ManagerId || model.managerId;
    const mName = model.ManagerFullName || model.managerFullName;
    if (mId) {
        this.selectManager(mId, mName);
    }

    // Render employees table (Step 4)
    const table = document.getElementById('selected-employees-table');
    if (table) {
        table.innerHTML = ''; // Clear table before rendering
        this.data.employees.forEach(emp => {
            this.renderEmployeeRow(emp);
        });
    }

    // Render files list
    const existing = model.ExistingFiles || model.existingFiles || [];
    this.data.existingFiles = existing;
    this.renderExistingFiles();
},

    // --- Manager and Team ---
    initManagerSearch() {
        const input = document.getElementById('manager-search-input');
        const list = document.getElementById('manager-results-list');
        input.oninput = async () => {
            if (input.value.length < 3) { list.style.display = 'none'; return; }
            const res = await fetch(`/api/employees/search?term=${input.value}`);
            const emps = await res.json();
            list.innerHTML = emps.map(e => `<li class="list-group-item list-group-item-action" onclick="wizard.selectManager(${e.id}, '${e.fullName}')">${e.fullName}</li>`).join('');
            list.style.display = 'block';
        };
    },

    selectManager(id, name) {
        this.data.managerId = id;
        document.getElementById('manager-name-display').innerText = name;
        document.getElementById('selected-manager-info').classList.remove('d-none');
        document.getElementById('manager-results-list').style.display = 'none';
    },

    initEmployeeSearch() {
        const input = document.getElementById('employee-search-input');
        const list = document.getElementById('search-results-list');
        input.oninput = async () => {
            if (input.value.length < 3) { list.style.display = 'none'; return; }
            const res = await fetch(`/api/employees/search?term=${input.value}`);
            const emps = await res.json();
            list.innerHTML = emps.filter(e => e.id !== this.data.managerId && !this.data.employees.some(ex => ex.id === e.id))
                .map(e => `<li class="list-group-item list-group-item-action" onclick='wizard.addEmployee(${JSON.stringify(e)})'>${e.fullName}</li>`).join('');
            list.style.display = 'block';
        };
    },

    addEmployee(emp) {
        this.data.employees.push(emp);
        this.renderEmployeeRow(emp);
        document.getElementById('employee-search-input').value = '';
        document.getElementById('search-results-list').style.display = 'none';
    },

    renderEmployeeRow(emp) {
    const id = emp.Id || emp.id;
    const name = emp.FullName || emp.fullName || 'Unknown';
    const email = emp.Email || emp.email || '-';

    const html = `
        <tr id="emp-row-${id}">
            <td>${name}</td>
            <td>${email}</td>
            <td class="text-center">
                <button type="button" class="btn btn-sm btn-outline-danger" onclick="wizard.removeEmployee(${id})">
                    <i class="bi bi-trash"></i> Remove
                </button>
            </td>
        </tr>`;
    
    const table = document.getElementById('selected-employees-table');
    if (table) {
        table.insertAdjacentHTML('beforeend', html);
    }
},

    removeEmployee(id) {
        console.log("Trying to remove employee with ID:", id);
        console.log("Array before removal:", this.data.employees);

        this.data.employees = this.data.employees.filter(emp => {
            const empId = emp.id !== undefined ? emp.id : emp.Id;
            return parseInt(empId) !== parseInt(id);
        });

        const row = document.getElementById(`emp-row-${id}`);
        if (row) {
            row.remove();
            console.log("Row removed from table.");
        } else {
            console.error("Table row not found! Looked for id: emp-row-" + id);
            // If not found by ID, try to find by structure (just in case)
            // But better check how row is created in renderEmployeeRow
        }

        this.saveToLocalStorage();
        console.log("Array after removal:", this.data.employees);
    },

    // --- Files ---
    initFileHandlers() {
        const zone = document.getElementById('drop-zone');
        const input = document.getElementById('project-files');
        zone.onclick = () => input.click();
        zone.ondragover = (e) => { e.preventDefault(); zone.classList.add('drop-zone--over'); };
        zone.ondragleave = () => zone.classList.remove('drop-zone--over');
        zone.ondrop = (e) => {
            e.preventDefault();
            zone.classList.remove('drop-zone--over');
            this.data.files = Array.from(e.dataTransfer.files);
            this.renderFileList();
        };
        input.onchange = () => { this.data.files = Array.from(input.files); this.renderFileList(); };
    },

    renderFileList() {
        document.getElementById('file-list').innerHTML = this.data.files.map(f => `<li class="list-group-item">${f.name}</li>`).join('');
    },

    renderExistingFiles() {
        const list = document.getElementById('existing-file-list');
        if (list) list.innerHTML = this.data.existingFiles.map(f => `<li class="list-group-item text-muted">${f} (On server)</li>`).join('');
    },

    // --- FINAL SUBMIT ---
    // Collect all form data and send to server
    async submit() {
        const btn = document.getElementById('submit-btn');
        btn.disabled = true;
        btn.innerText = "Submitting...";

        const formData = new FormData();
        if (this.data.id > 0) formData.append('Id', this.data.id);
        
        formData.append('Name', document.getElementById('Name').value);
        formData.append('StartDate', document.getElementById('StartDate').value);
        formData.append('EndDate', document.getElementById('EndDate').value);
        formData.append('Priority', document.getElementById('Priority').value);
        formData.append('CustomerCompany', document.getElementById('CustomerCompany').value);
        formData.append('ExecutorCompany', document.getElementById('ExecutorCompany').value);
        formData.append('ManagerId', this.data.managerId);

        // Use safe loop to append all employee IDs (handles both 'id' and 'Id')
        this.data.employees.forEach(emp => {
            // Check both 'id' and 'Id' property names
            const id = emp.id !== undefined ? emp.id : (emp.Id !== undefined ? emp.Id : null);
            
            if (id !== null && id !== undefined && id !== "undefined") {
                formData.append('EmployeeIds', id);
            } else {
                console.error("Critical error: employee has no ID!", emp);
            }
        });
        // ---------------------------------------------------------
        this.data.files.forEach(file => formData.append('Documents', file));

        const url = this.data.id > 0 ? `/api/projects/edit/${this.data.id}` : '/api/projects/create';

        try {
            const response = await fetch(url, { method: 'POST', body: formData });
            if (response.ok) {
                localStorage.clear();
                alert("Success!");
                window.location.href = '/Projects';
            } else {
                alert("Server error: " + await response.text());
                btn.disabled = false;
            }
        } catch (err) {
            alert("Network error!");
            btn.disabled = false;
        }
    },

    saveToLocalStorage() {
        const backup = { ...this.data };
        delete backup.files;
        localStorage.setItem('wizard_backup', JSON.stringify(backup));
    },

    loadFromLocalStorage() {
        const saved = localStorage.getItem('wizard_backup');
        if (saved) {
            this.data = JSON.parse(saved);
            this.currentStep = parseInt(localStorage.getItem('wizard_step')) || 1;
            
            // Fill fields from localStorage backup
            document.getElementById('Name').value = this.data.name || '';
            document.getElementById('StartDate').value = this.data.startDate || '';
            document.getElementById('EndDate').value = this.data.endDate || '';
            document.getElementById('Priority').value = this.data.priority || 0;
            document.getElementById('CustomerCompany').value = this.data.customerCompany || '';
            document.getElementById('ExecutorCompany').value = this.data.executorCompany || '';
            
            if (this.data.managerId) this.selectManager(this.data.managerId, this.data.managerName);
            this.data.employees.forEach(e => this.renderEmployeeRow(e));
        }
    }
};

// Initialize wizard logic on page load

document.addEventListener('DOMContentLoaded', () => wizard.init());