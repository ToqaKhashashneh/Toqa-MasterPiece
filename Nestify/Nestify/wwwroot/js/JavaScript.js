
        // This would be replaced with actual API calls in a real implementation
        document.addEventListener('DOMContentLoaded', function() {
            // Initialize dashboard stats
            updateDashboardStats();
            
            // Initialize all tables
            initializeUsersTable();
            initializePaymentsTable();
            initializePackagesTable();
            initializePropertiesTable();
            initializeInquiriesTable();
            initializeInteriorTable();
            initializeContactTable();
            initializeLocationsTable();
            initializeSublocationsTable();
            
            // Set up event listeners
            setupEventListeners();
            
            // Mobile sidebar toggle
            document.getElementById('sidebarToggle').addEventListener('click', function() {
                document.getElementById('sidebar').classList.toggle('show');
            });
            
            // Close sidebar when clicking outside on mobile
            document.addEventListener('click', function(event) {
                const sidebar = document.getElementById('sidebar');
                const sidebarToggle = document.getElementById('sidebarToggle');
                
                if (window.innerWidth <= 992 && 
                    !sidebar.contains(event.target) && 
                    event.target !== sidebarToggle &&
                    !sidebarToggle.contains(event.target)) {
                    sidebar.classList.remove('show');
                }
            });
        });
        
        function updateDashboardStats() {
            // Simulate API calls to get stats
            document.getElementById('total-users').textContent = '125';
            document.getElementById('total-properties').textContent = '87';
            document.getElementById('pending-payments').textContent = '5';
            document.getElementById('new-inquiries').textContent = '12';
            
            // Populate recent properties table
            const recentProperties = [
                { id: 1, name: 'Luxury Villa', price: '$1,250,000', status: 'For Sale', date: '2023-06-15' },
                { id: 2, name: 'Downtown Apartment', price: '$2,500/mo', status: 'For Rent', date: '2023-06-14' },
                { id: 3, name: 'Countryside House', price: '$850,000', status: 'Sold', date: '2023-06-12' },
                { id: 4, name: 'Beachfront Condo', price: '$1,750,000', status: 'For Sale', date: '2023-06-10' }
            ];
            
            const recentPropertiesTable = document.getElementById('recent-properties');
            recentPropertiesTable.innerHTML = '';
            recentProperties.forEach(property => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${property.name}</td>
                    <td>${property.price}</td>
                    <td>${property.status}</td>
                    <td>${property.date}</td>
                `;
                recentPropertiesTable.appendChild(row);
            });
            
            // Populate recent payments table
            const recentPayments = [
                { id: 1, user: 'John Doe', amount: '$25.00', status: 'Completed', date: '2023-06-15' },
                { id: 2, user: 'Jane Smith', amount: '$60.00', status: 'Completed', date: '2023-06-14' },
                { id: 3, user: 'Robert Johnson', amount: '$10.00', status: 'Pending', date: '2023-06-12' },
                { id: 4, user: 'Emily Davis', amount: '$25.00', status: 'Failed', date: '2023-06-10' }
            ];
            
            const recentPaymentsTable = document.getElementById('recent-payments');
            recentPaymentsTable.innerHTML = '';
            recentPayments.forEach(payment => {
                const statusClass = payment.status === 'Completed' ? 'badge-completed' : 
                                  payment.status === 'Pending' ? 'badge-pending' : 'badge-failed';
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${payment.user}</td>
                    <td>${payment.amount}</td>
                    <td><span class="badge ${statusClass}">${payment.status}</span></td>
                    <td>${payment.date}</td>
                `;
                recentPaymentsTable.appendChild(row);
            });
        }
        
        function initializeUsersTable() {
            // Simulate API call to get users
            const users = [
                { id: 1, name: 'John Doe', email: 'john@example.com', phone: '123-456-7890', role: 'User', business: 'Yes', verified: 'No' },
                { id: 2, name: 'Jane Smith', email: 'jane@example.com', phone: '987-654-3210', role: 'Admin', business: 'No', verified: 'Yes' },
                { id: 3, name: 'Robert Johnson', email: 'robert@example.com', phone: '555-123-4567', role: 'User', business: 'Yes', verified: 'Yes' },
                { id: 4, name: 'Emily Davis', email: 'emily@example.com', phone: '444-555-6666', role: 'User', business: 'No', verified: 'No' },
                { id: 5, name: 'Michael Brown', email: 'michael@example.com', phone: '111-222-3333', role: 'Agent', business: 'Yes', verified: 'Yes' },
                { id: 6, name: 'Sarah Wilson', email: 'sarah@example.com', phone: '777-888-9999', role: 'User', business: 'No', verified: 'No' }
            ];
            
            const usersTable = document.getElementById('users-table').getElementsByTagName('tbody')[0];
            usersTable.innerHTML = '';
            users.forEach(user => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${user.id}</td>
                    <td>${user.name}</td>
                    <td>${user.email}</td>
                    <td>${user.phone}</td>
                    <td>${user.role}</td>
                    <td>${user.business}</td>
                    <td>${user.verified}</td>
                    <td>
                        <button class="btn btn-sm ${user.verified === 'Yes' ? 'btn-outline-secondary' : 'btn-outline-primary'}" 
                                onclick="toggleUserVerification(${user.id}, ${user.verified === 'Yes'})">
                            ${user.verified === 'Yes' ? 'Unverify' : 'Verify'}
                        </button>
                        <button class="btn btn-sm btn-info" onclick="viewUserDetails(${user.id})">
                            <i class="bi bi-eye"></i>
                        </button>
                    </td>
                `;
                usersTable.appendChild(row);
            });
            
            // Set up filter event listeners
            document.getElementById('userSearch').addEventListener('input', filterUsers);
            document.getElementById('userRoleFilter').addEventListener('change', filterUsers);
            document.getElementById('userVerificationFilter').addEventListener('change', filterUsers);
            document.getElementById('searchUsersBtn').addEventListener('click', filterUsers);
        }
        
        function filterUsers() {
            const searchTerm = document.getElementById('userSearch').value.toLowerCase();
            const roleFilter = document.getElementById('userRoleFilter').value;
            const verificationFilter = document.getElementById('userVerificationFilter').value;
            
            const rows = document.querySelectorAll('#users-table tbody tr');
            
            rows.forEach(row => {
                const name = row.cells[1].textContent.toLowerCase();
                const email = row.cells[2].textContent.toLowerCase();
                const role = row.cells[4].textContent;
                const verified = row.cells[6].textContent;
                
                const matchesSearch = name.includes(searchTerm) || email.includes(searchTerm);
                const matchesRole = !roleFilter || role === roleFilter;
                const matchesVerification = 
                    !verificationFilter || 
                    (verificationFilter === 'verified' && verified === 'Yes') || 
                    (verificationFilter === 'unverified' && verified === 'No');
                
                if (matchesSearch && matchesRole && matchesVerification) {
                    row.style.display = '';
                } else {
                    row.style.display = 'none';
                }
            });
        }
        
        function initializePaymentsTable() {
            // Simulate API call to get payments
            const payments = [
                { id: 1, user: 'John Doe', amount: '$25.00', method: 'PayPal', package: 'Standard', status: 'Completed', date: '2023-06-15' },
                { id: 2, user: 'Jane Smith', amount: '$60.00', method: 'Credit Card', package: 'Pro', status: 'Completed', date: '2023-06-14' },
                { id: 3, user: 'Robert Johnson', amount: '$10.00', method: 'Bank Transfer', package: 'Basic', status: 'Pending', date: '2023-06-12' },
                { id: 4, user: 'Emily Davis', amount: '$25.00', method: 'PayPal', package: 'Standard', status: 'Failed', date: '2023-06-10' }
            ];
            
            const paymentsTable = document.getElementById('payments-table').getElementsByTagName('tbody')[0];
            paymentsTable.innerHTML = '';
            payments.forEach(payment => {
                const statusClass = payment.status === 'Completed' ? 'badge-completed' : 
                                  payment.status === 'Pending' ? 'badge-pending' : 'badge-failed';
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${payment.id}</td>
                    <td>${payment.user}</td>
                    <td>${payment.amount}</td>
                    <td>${payment.method}</td>
                    <td>${payment.package}</td>
                    <td><span class="badge ${statusClass}">${payment.status}</span></td>
                    <td>${payment.date}</td>
                    <td>
                        ${payment.status === 'Pending' ? `
                        <button class="btn btn-sm btn-success" onclick="updatePaymentStatus(${payment.id}, 'Completed')">Approve</button>
                        <button class="btn btn-sm btn-danger" onclick="updatePaymentStatus(${payment.id}, 'Failed')">Reject</button>
                        ` : ''}
                    </td>
                `;
                paymentsTable.appendChild(row);
            });
        }
        
        function initializePackagesTable() {
            // Simulate API call to get packages
            const packages = [
                { id: 1, name: 'Basic', price: '$10.00', postLimit: '2', duration: '30' },
                { id: 2, name: 'Standard', price: '$25.00', postLimit: '5', duration: '30' },
                { id: 3, name: 'Pro', price: '$60.00', postLimit: '15', duration: '90' }
            ];
            
            const packagesTable = document.getElementById('packages-table').getElementsByTagName('tbody')[0];
            packagesTable.innerHTML = '';
            packages.forEach(pkg => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${pkg.id}</td>
                    <td>${pkg.name}</td>
                    <td>${pkg.price}</td>
                    <td>${pkg.postLimit}</td>
                    <td>${pkg.duration}</td>
                    <td>
                        <button class="btn btn-sm btn-primary" onclick="editPackage(${pkg.id})">
                            <i class="bi bi-pencil"></i> Edit
                        </button>
                        <button class="btn btn-sm btn-danger" onclick="confirmDeletePackage(${pkg.id})">
                            <i class="bi bi-trash"></i> Delete
                        </button>
                    </td>
                `;
                packagesTable.appendChild(row);
            });
        }
        
        function initializePropertiesTable() {
            // Simulate API call to get properties with additional data
            const properties = [
                { 
                    id: 1, 
                    name: 'Luxury Villa', 
                    owner: 'John Doe', 
                    location: 'Downtown', 
                    price: '$1,250,000', 
                    status: 'For Sale', 
                    featured: 'Yes', 
                    visible: 'Yes',
                    features: ['Air Conditioning', 'Swimming Pool', 'Garden', 'Security']
                },
                { 
                    id: 2, 
                    name: 'Modern Apartment', 
                    owner: 'Jane Smith', 
                    location: 'Uptown', 
                    price: '$2,500/mo', 
                    status: 'For Rent', 
                    featured: 'No', 
                    visible: 'Yes',
                    features: ['Elevator', 'Gym', 'Parking']
                },
                { 
                    id: 3, 
                    name: 'Countryside House', 
                    owner: 'Robert Johnson', 
                    location: 'Suburbs', 
                    price: '$850,000', 
                    status: 'Sold', 
                    featured: 'No', 
                    visible: 'No',
                    features: ['Fireplace', 'Garden']
                },
                { 
                    id: 4, 
                    name: 'Beachfront Condo', 
                    owner: 'Emily Davis', 
                    location: 'Coastal', 
                    price: '$1,750,000', 
                    status: 'For Sale', 
                    featured: 'Yes', 
                    visible: 'Yes',
                    features: ['Balcony', 'Air Conditioning', 'Parking']
                }
            ];
            
            const propertiesTable = document.getElementById('properties-table').getElementsByTagName('tbody')[0];
            propertiesTable.innerHTML = '';
            properties.forEach(property => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${property.id}</td>
                    <td>${property.name}</td>
                    <td>${property.owner}</td>
                    <td>${property.location}</td>
                    <td>${property.price}</td>
                    <td>${property.status}</td>
                    <td>
                        <div class="form-check form-switch">
                            <input class="form-check-input" type="checkbox" ${property.featured === 'Yes' ? 'checked' : ''} 
                                   onchange="togglePropertyFeatured(${property.id}, this.checked)">
                        </div>
                    </td>
                    <td>
                        <div class="form-check form-switch">
                            <input class="form-check-input" type="checkbox" ${property.visible === 'Yes' ? 'checked' : ''} 
                                   onchange="togglePropertyVisibility(${property.id}, this.checked)">
                        </div>
                    </td>
                    <td>
                        <button class="btn btn-sm btn-primary" onclick="editProperty(${property.id})">
                            <i class="bi bi-pencil"></i> Edit
                        </button>
                        <button class="btn btn-sm btn-info" onclick="showPropertyFeatures(${property.id}, 'preview')">
                            <i class="bi bi-list-check"></i> Features
                        </button>
                        <button class="btn btn-sm btn-danger" onclick="confirmDeleteProperty(${property.id})">
                            <i class="bi bi-trash"></i> Delete
                        </button>
                    </td>
                `;
                propertiesTable.appendChild(row);
            });
            
            // Set up filter event listeners
            document.getElementById('propertyStatusFilter').addEventListener('change', filterProperties);
            document.getElementById('propertyFeaturedFilter').addEventListener('change', filterProperties);
            document.getElementById('propertyVisibilityFilter').addEventListener('change', filterProperties);
            document.getElementById('propertySearch').addEventListener('input', filterProperties);
            document.getElementById('searchPropertiesBtn').addEventListener('click', filterProperties);
        }
        
        function filterProperties() {
            const statusFilter = document.getElementById('propertyStatusFilter').value;
            const featuredFilter = document.getElementById('propertyFeaturedFilter').value;
            const visibilityFilter = document.getElementById('propertyVisibilityFilter').value;
            const searchTerm = document.getElementById('propertySearch').value.toLowerCase();
            
            const rows = document.querySelectorAll('#properties-table tbody tr');
            
            rows.forEach(row => {
                const name = row.cells[1].textContent.toLowerCase();
                const owner = row.cells[2].textContent.toLowerCase();
                const location = row.cells[3].textContent.toLowerCase();
                const status = row.cells[5].textContent;
                const featured = row.cells[6].querySelector('input').checked;
                const visible = row.cells[7].querySelector('input').checked;
                
                const matchesSearch = name.includes(searchTerm) || owner.includes(searchTerm) || location.includes(searchTerm);
                const matchesStatus = !statusFilter || status === statusFilter;
                const matchesFeatured = 
                    !featuredFilter || 
                    (featuredFilter === 'featured' && featured) || 
                    (featuredFilter === 'not-featured' && !featured);
                const matchesVisibility = 
                    !visibilityFilter || 
                    (visibilityFilter === 'visible' && visible) || 
                    (visibilityFilter === 'hidden' && !visible);
                
                if (matchesSearch && matchesStatus && matchesFeatured && matchesVisibility) {
                    row.style.display = '';
                } else {
                    row.style.display = 'none';
                }
            });
        }
        
        // List of all possible property features
        const allPropertyFeatures = [
            'Air Conditioning', 'Heating', 'Parking', 'Balcony', 'Garden',
            'Swimming Pool', 'Gym', 'Security', 'Fireplace', 'Elevator',
            'WiFi', 'Cable TV', 'Laundry', 'Dishwasher', 'Microwave',
            'Refrigerator', 'Oven', 'Furnished', 'Pet Friendly', 'Wheelchair Access'
        ];
        
        // Currently selected property ID for features editing
        let currentPropertyIdForFeatures = null;
        
        function showPropertyFeatures(propertyId, mode = 'preview') {
            // In a real app, you would fetch the property features from your API
            const properties = [
                { 
                    id: 1, 
                    features: ['Air Conditioning', 'Swimming Pool', 'Garden', 'Security']
                },
                { 
                    id: 2, 
                    features: ['Elevator', 'Gym', 'Parking']
                },
                { 
                    id: 3, 
                    features: ['Fireplace', 'Garden']
                },
                { 
                    id: 4, 
                    features: ['Balcony', 'Air Conditioning', 'Parking']
                }
            ];
            
            const property = properties.find(p => p.id === propertyId);
            if (property) {
                currentPropertyIdForFeatures = propertyId;
                
                if (mode === 'preview') {
                    // Show preview modal
                    showFeaturesPreviewModal(property.features);
                } else {
                    // Show edit modal
                    showFeaturesEditModal(property.features);
                }
            }
        }
        
        function showFeaturesPreviewModal(features) {
            const featuresList = document.getElementById('featuresPreviewList');
            featuresList.innerHTML = '';
            
            if (features.length === 0) {
                featuresList.innerHTML = '<p>No features added for this property.</p>';
            } else {
                features.forEach(feature => {
                    const badge = document.createElement('span');
                    badge.className = 'badge bg-primary feature-badge';
                    badge.textContent = feature;
                    featuresList.appendChild(badge);
                });
            }
            
            // Show the preview modal
            const previewModal = new bootstrap.Modal(document.getElementById('propertyFeaturesPreviewModal'));
            previewModal.show();
        }
        
        function showEditFeaturesModal() {
            // Close the preview modal first
            bootstrap.Modal.getInstance(document.getElementById('propertyFeaturesPreviewModal')).hide();
            
            // Get the current property's features
            const properties = [
                { 
                    id: 1, 
                    features: ['Air Conditioning', 'Swimming Pool', 'Garden', 'Security']
                },
                { 
                    id: 2, 
                    features: ['Elevator', 'Gym', 'Parking']
                },
                { 
                    id: 3, 
                    features: ['Fireplace', 'Garden']
                },
                { 
                    id: 4, 
                    features: ['Balcony', 'Air Conditioning', 'Parking']
                }
            ];
            
            const property = properties.find(p => p.id === currentPropertyIdForFeatures);
            if (property) {
                showFeaturesEditModal(property.features);
            }
        }
        
        function showFeaturesEditModal(currentFeatures) {
            const featuresEditList = document.getElementById('featuresEditList');
            featuresEditList.innerHTML = '';
            
            allPropertyFeatures.forEach(feature => {
                const isChecked = currentFeatures.includes(feature);
                
                const featureItem = document.createElement('div');
                featureItem.className = 'feature-edit-item';
                
                const checkbox = document.createElement('input');
                checkbox.type = 'checkbox';
                checkbox.id = `editFeature_${feature.replace(/\s+/g, '_')}`;
                checkbox.checked = isChecked;
                checkbox.value = feature;
                
                const label = document.createElement('label');
                label.htmlFor = `editFeature_${feature.replace(/\s+/g, '_')}`;
                label.textContent = feature;
                
                featureItem.appendChild(checkbox);
                featureItem.appendChild(label);
                featuresEditList.appendChild(featureItem);
            });
            
            // Show the edit modal
            const editModal = new bootstrap.Modal(document.getElementById('propertyFeaturesEditModal'));
            editModal.show();
        }
        
        function savePropertyFeatures() {
            const checkboxes = document.querySelectorAll('#featuresEditList input[type="checkbox"]:checked');
            const selectedFeatures = Array.from(checkboxes).map(checkbox => checkbox.value);
            
            console.log(`Saving features for property ${currentPropertyIdForFeatures}:`, selectedFeatures);
            
            // Here you would make an API call to save the features
            // For now, we'll just show a success message
            alert('Property features updated successfully!');
            
            // Close the edit modal
            bootstrap.Modal.getInstance(document.getElementById('propertyFeaturesEditModal')).hide();
            
            // Update the preview in case they want to see it again
            showPropertyFeatures(currentPropertyIdForFeatures, 'preview');
        }
        
        function initializeInquiriesTable() {
            // Simulate API call to get inquiries
            const inquiries = [
                { id: 1, property: 'Luxury Villa', name: 'Michael Brown', email: 'michael@example.com', message: 'I\'m interested in this property...', status: 'Pending', date: '2023-06-15' },
                { id: 2, property: 'Modern Apartment', name: 'Sarah Wilson', email: 'sarah@example.com', message: 'Can I schedule a viewing?', status: 'Confirmed', date: '2023-06-14' },
                { id: 3, property: 'Countryside House', name: 'David Miller', email: 'david@example.com', message: 'Is this still available?', status: 'Completed', date: '2023-06-12' },
                { id: 4, property: 'Beachfront Condo', name: 'Lisa Taylor', email: 'lisa@example.com', message: 'Please send more details', status: 'Pending', date: '2023-06-10' }
            ];
            
            const inquiriesTable = document.getElementById('inquiries-table').getElementsByTagName('tbody')[0];
            inquiriesTable.innerHTML = '';
            inquiries.forEach(inquiry => {
                const statusClass = inquiry.status === 'Pending' ? 'badge-pending' : 
                                  inquiry.status === 'Confirmed' ? 'badge-primary' : 'badge-completed';
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${inquiry.id}</td>
                    <td>${inquiry.property}</td>
                    <td>${inquiry.name}</td>
                    <td>${inquiry.email}</td>
                    <td>${inquiry.message.substring(0, 30)}...</td>
                    <td><span class="badge ${statusClass}">${inquiry.status}</span></td>
                    <td>${inquiry.date}</td>
                    <td>
                        ${inquiry.status === 'Pending' ? `
                        <button class="btn btn-sm btn-success" onclick="updateInquiryStatus(${inquiry.id}, 'Confirmed')">Accept</button>
                        <button class="btn btn-sm btn-danger" onclick="updateInquiryStatus(${inquiry.id}, 'Cancelled')">Reject</button>
                        ` : ''}
                        <button class="btn btn-sm btn-info" onclick="viewInquiryDetails(${inquiry.id})">
                            <i class="bi bi-eye"></i>
                        </button>
                    </td>
                `;
                inquiriesTable.appendChild(row);
            });
        }
        
        function initializeInteriorTable() {
            // Simulate API call to get interior design inquiries
            const interiorInquiries = [
                { id: 1, name: 'Michael Brown', email: 'michael@example.com', phone: '123-456-7890', message: 'I need help with my living room design...', date: '2023-06-15' },
                { id: 2, name: 'Sarah Wilson', email: 'sarah@example.com', phone: '987-654-3210', message: 'Looking for a full home redesign', date: '2023-06-14' },
                { id: 3, name: 'David Miller', email: 'david@example.com', phone: '555-123-4567', message: 'Consultation for office space', date: '2023-06-12' },
                { id: 4, name: 'Lisa Taylor', email: 'lisa@example.com', phone: '444-555-6666', message: 'Kitchen remodeling inquiry', date: '2023-06-10' }
            ];
            
            const interiorTable = document.getElementById('interior-table').getElementsByTagName('tbody')[0];
            interiorTable.innerHTML = '';
            interiorInquiries.forEach(inquiry => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${inquiry.id}</td>
                    <td>${inquiry.name}</td>
                    <td>${inquiry.email}</td>
                    <td>${inquiry.phone}</td>
                    <td>${inquiry.message.substring(0, 30)}...</td>
                    <td>${inquiry.date}</td>
                    <td>
                        <button class="btn btn-sm btn-info" onclick="viewInteriorInquiry(${inquiry.id})">
                            <i class="bi bi-eye"></i> View
                        </button>
                        <button class="btn btn-sm btn-danger" onclick="deleteInteriorInquiry(${inquiry.id})">
                            <i class="bi bi-trash"></i> Delete
                        </button>
                    </td>
                `;
                interiorTable.appendChild(row);
            });
        }
        
        function initializeContactTable() {
            // Simulate API call to get contact inquiries
            const contactInquiries = [
                { id: 1, name: 'Michael Brown', email: 'michael@example.com', phone: '123-456-7890', message: 'General question about your services...', date: '2023-06-15' },
                { id: 2, name: 'Sarah Wilson', email: 'sarah@example.com', phone: '987-654-3210', message: 'Looking for partnership opportunities', date: '2023-06-14' },
                { id: 3, name: 'David Miller', email: 'david@example.com', phone: '555-123-4567', message: 'Feedback about your website', date: '2023-06-12' },
                { id: 4, name: 'Lisa Taylor', email: 'lisa@example.com', phone: '444-555-6666', message: 'Press inquiry', date: '2023-06-10' }
            ];
            
            const contactTable = document.getElementById('contact-table').getElementsByTagName('tbody')[0];
            contactTable.innerHTML = '';
            contactInquiries.forEach(inquiry => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${inquiry.id}</td>
                    <td>${inquiry.name}</td>
                    <td>${inquiry.email}</td>
                    <td>${inquiry.phone}</td>
                    <td>${inquiry.message.substring(0, 30)}...</td>
                    <td>${inquiry.date}</td>
                    <td>
                        <button class="btn btn-sm btn-primary" onclick="addToTestimonials(${inquiry.id})">
                            <i class="bi bi-chat-square-quote"></i> Testimonial
                        </button>
                        <button class="btn btn-sm btn-info" onclick="viewContactInquiry(${inquiry.id})">
                            <i class="bi bi-eye"></i> View
                        </button>
                        <button class="btn btn-sm btn-danger" onclick="deleteContactInquiry(${inquiry.id})">
                            <i class="bi bi-trash"></i> Delete
                        </button>
                    </td>
                `;
                contactTable.appendChild(row);
            });
        }
        
        function initializeLocationsTable() {
            // Simulate API call to get locations
            const locations = [
                { id: 1, name: 'Downtown' },
                { id: 2, name: 'Uptown' },
                { id: 3, name: 'Suburbs' },
                { id: 4, name: 'Coastal' }
            ];
            
            const locationsTable = document.getElementById('locations-table').getElementsByTagName('tbody')[0];
            locationsTable.innerHTML = '';
            locations.forEach(location => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${location.id}</td>
                    <td>${location.name}</td>
                    <td>
                        <button class="btn btn-sm btn-primary" onclick="editLocation(${location.id})">
                            <i class="bi bi-pencil"></i> Edit
                        </button>
                        <button class="btn btn-sm btn-danger" onclick="confirmDeleteLocation(${location.id})">
                            <i class="bi bi-trash"></i> Delete
                        </button>
                    </td>
                `;
                locationsTable.appendChild(row);
            });
        }
        
        function initializeSublocationsTable() {
            // Simulate API call to get sublocations
            const sublocations = [
                { id: 1, name: 'Financial District', location: 'Downtown' },
                { id: 2, name: 'Arts Quarter', location: 'Uptown' },
                { id: 3, name: 'Green Valley', location: 'Suburbs' },
                { id: 4, name: 'Ocean View', location: 'Coastal' }
            ];
            
            const sublocationsTable = document.getElementById('sublocations-table').getElementsByTagName('tbody')[0];
            sublocationsTable.innerHTML = '';
            sublocations.forEach(sublocation => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${sublocation.id}</td>
                    <td>${sublocation.name}</td>
                    <td>${sublocation.location}</td>
                    <td>
                        <button class="btn btn-sm btn-primary" onclick="editSublocation(${sublocation.id})">
                            <i class="bi bi-pencil"></i> Edit
                        </button>
                        <button class="btn btn-sm btn-danger" onclick="confirmDeleteSublocation(${sublocation.id})">
                            <i class="bi bi-trash"></i> Delete
                        </button>
                    </td>
                `;
                sublocationsTable.appendChild(row);
            });
        }
        
        function setupEventListeners() {
            // Package form submission
            document.getElementById('savePackage').addEventListener('click', function() {
                const name = document.getElementById('packageName').value;
                const price = document.getElementById('packagePrice').value;
                const postLimit = document.getElementById('postLimit').value;
                const durationDays = document.getElementById('durationDays').value;
                
                // Here you would make an API call to save the package
                console.log('Saving package:', { name, price, postLimit, durationDays });
                
                // Close modal and refresh table
                bootstrap.Modal.getInstance(document.getElementById('addPackageModal')).hide();
                initializePackagesTable();
            });
            
            // Update package form submission
            document.getElementById('updatePackage').addEventListener('click', function() {
                const id = document.getElementById('editPackageId').value;
                const name = document.getElementById('editPackageName').value;
                const price = document.getElementById('editPackagePrice').value;
                const postLimit = document.getElementById('editPostLimit').value;
                const durationDays = document.getElementById('editDurationDays').value;
                
                // Here you would make an API call to update the package
                console.log('Updating package:', { id, name, price, postLimit, durationDays });
                
                // Close modal and refresh table
                bootstrap.Modal.getInstance(document.getElementById('editPackageModal')).hide();
                initializePackagesTable();
            });
            
            // Property form submission
            document.getElementById('saveProperty').addEventListener('click', function() {
                // Collect all property data from the form
                const propertyData = {
                    name: document.getElementById('propertyName').value,
                    owner: document.getElementById('propertyOwner').value,
                    location: document.getElementById('propertyLocation').value,
                    sublocation: document.getElementById('propertySublocation').value,
                    bedrooms: document.getElementById('propertyBedrooms').value,
                    bathrooms: document.getElementById('propertyBathrooms').value,
                    size: document.getElementById('propertySize').value,
                    price: document.getElementById('propertyPrice').value,
                    status: document.getElementById('propertyStatus').value,
                    yearBuilt: document.getElementById('propertyYearBuilt').value,
                    lotArea: document.getElementById('propertyLotArea').value,
                    lotDimensions: document.getElementById('propertyLotDimensions').value,
                    description: document.getElementById('propertyDescription').value,
                    videoURL: document.getElementById('propertyVideoURL').value,
                    featured: document.getElementById('propertyFeatured').checked,
                    visible: document.getElementById('propertyVisible').checked
                };
                
                console.log('Saving property:', propertyData);
                
                // Here you would make an API call to save the property
                // For now, we'll just close the modal and refresh the table
                bootstrap.Modal.getInstance(document.getElementById('addPropertyModal')).hide();
                initializePropertiesTable();
            });
            
            // Location form submission
            document.getElementById('saveLocation').addEventListener('click', function() {
                const name = document.getElementById('locationName').value;
                
                // Here you would make an API call to save the location
                console.log('Saving location:', { name });
                
                // Close modal and refresh table
                bootstrap.Modal.getInstance(document.getElementById('addLocationModal')).hide();
                initializeLocationsTable();
            });
            
            // Sublocation form submission
            document.getElementById('saveSublocation').addEventListener('click', function() {
                const name = document.getElementById('sublocationName').value;
                const parentId = document.getElementById('sublocationParent').value;
                
                // Here you would make an API call to save the sublocation
                console.log('Saving sublocation:', { name, parentId });
                
                // Close modal and refresh table
                bootstrap.Modal.getInstance(document.getElementById('addSublocationModal')).hide();
                initializeSublocationsTable();
            });
        }
        
        // Package edit function
        function editPackage(packageId) {
            // In a real app, you would fetch the package data from your API
            const packages = [
                { id: 1, name: 'Basic', price: '10.00', postLimit: '2', duration: '30' },
                { id: 2, name: 'Standard', price: '25.00', postLimit: '5', duration: '30' },
                { id: 3, name: 'Pro', price: '60.00', postLimit: '15', duration: '90' }
            ];
            
            const pkg = packages.find(p => p.id === packageId);
            if (pkg) {
                document.getElementById('editPackageId').value = pkg.id;
                document.getElementById('editPackageName').value = pkg.name;
                document.getElementById('editPackagePrice').value = pkg.price;
                document.getElementById('editPostLimit').value = pkg.postLimit;
                document.getElementById('editDurationDays').value = pkg.duration;
                
                // Show the edit modal
                const editModal = new bootstrap.Modal(document.getElementById('editPackageModal'));
                editModal.show();
            }
        }
        
        // Location edit function
        function editLocation(locationId) {
            // In a real app, you would fetch the location data from your API
            const locations = [
                { id: 1, name: 'Downtown' },
                { id: 2, name: 'Uptown' },
                { id: 3, name: 'Suburbs' },
                { id: 4, name: 'Coastal' }
            ];
            
            const location = locations.find(l => l.id === locationId);
            if (location) {
                // For simplicity, we're reusing the add modal as an edit modal
                document.getElementById('locationName').value = location.name;
                
                // Change the modal title and save button text
                document.getElementById('addLocationModalLabel').textContent = 'Edit Location';
                document.getElementById('saveLocation').textContent = 'Update Location';
                
                // Show the modal
                const locationModal = new bootstrap.Modal(document.getElementById('addLocationModal'));
                locationModal.show();
            }
        }
        
        // Sublocation edit function
        function editSublocation(sublocationId) {
            // In a real app, you would fetch the sublocation data from your API
            const sublocations = [
                { id: 1, name: 'Financial District', locationId: 1 },
                { id: 2, name: 'Arts Quarter', locationId: 2 },
                { id: 3, name: 'Green Valley', locationId: 3 },
                { id: 4, name: 'Ocean View', locationId: 4 }
            ];
            
            const sublocation = sublocations.find(s => s.id === sublocationId);
            if (sublocation) {
                // For simplicity, we're reusing the add modal as an edit modal
                document.getElementById('sublocationName').value = sublocation.name;
                document.getElementById('sublocationParent').value = sublocation.locationId;
                
                // Change the modal title and save button text
                document.getElementById('addSublocationModalLabel').textContent = 'Edit Sublocation';
                document.getElementById('saveSublocation').textContent = 'Update Sublocation';
                
                // Show the modal
                const sublocationModal = new bootstrap.Modal(document.getElementById('addSublocationModal'));
                sublocationModal.show();
            }
        }
        
        // Property edit function
        function editProperty(propertyId) {
            // In a real app, you would fetch the property data from your API
            const properties = [
                { 
                    id: 1, 
                    name: 'Luxury Villa', 
                    ownerId: 1, 
                    locationId: 1, 
                    sublocationId: 1,
                    bedrooms: 4,
                    bathrooms: 3,
                    size: 2500,
                    price: 1250000,
                    status: 'For Sale',
                    yearBuilt: 2015,
                    lotArea: 0.5,
                    lotDimensions: '100x200',
                    description: 'A beautiful luxury villa with modern amenities',
                    videoURL: '',
                    featured: true,
                    visible: true
                },
                // Add other properties as needed
            ];
            
            const property = properties.find(p => p.id === propertyId);
            if (property) {
                // For simplicity, we're reusing the add modal as an edit modal
                document.getElementById('propertyName').value = property.name;
                document.getElementById('propertyOwner').value = property.ownerId;
                document.getElementById('propertyLocation').value = property.locationId;
                document.getElementById('propertySublocation').value = property.sublocationId;
                document.getElementById('propertyBedrooms').value = property.bedrooms;
                document.getElementById('propertyBathrooms').value = property.bathrooms;
                document.getElementById('propertySize').value = property.size;
                document.getElementById('propertyPrice').value = property.price;
                document.getElementById('propertyStatus').value = property.status;
                document.getElementById('propertyYearBuilt').value = property.yearBuilt;
                document.getElementById('propertyLotArea').value = property.lotArea;
                document.getElementById('propertyLotDimensions').value = property.lotDimensions;
                document.getElementById('propertyDescription').value = property.description;
                document.getElementById('propertyVideoURL').value = property.videoURL;
                document.getElementById('propertyFeatured').checked = property.featured;
                document.getElementById('propertyVisible').checked = property.visible;
                
                // Change the modal title and save button text
                document.getElementById('addPropertyModalLabel').textContent = 'Edit Property';
                document.getElementById('saveProperty').textContent = 'Update Property';
                
                // Show the modal
                const propertyModal = new bootstrap.Modal(document.getElementById('addPropertyModal'));
                propertyModal.show();
            }
        }
        
        // Confirmation functions
        function confirmDeletePackage(packageId) {
            document.getElementById('confirmationMessage').textContent = `Are you sure you want to delete package #${packageId}?`;
            document.getElementById('confirmAction').onclick = function() {
                deletePackage(packageId);
                bootstrap.Modal.getInstance(document.getElementById('confirmationModal')).hide();
            };
            bootstrap.Modal.getInstance(document.getElementById('confirmationModal')).show();
        }
        
        function confirmDeleteProperty(propertyId) {
            document.getElementById('confirmationMessage').textContent = `Are you sure you want to delete property #${propertyId}?`;
            document.getElementById('confirmAction').onclick = function() {
                deleteProperty(propertyId);
                bootstrap.Modal.getInstance(document.getElementById('confirmationModal')).hide();
            };
            bootstrap.Modal.getInstance(document.getElementById('confirmationModal')).show();
        }
        
        function confirmDeleteLocation(locationId) {
            document.getElementById('confirmationMessage').textContent = `Are you sure you want to delete location #${locationId}?`;
            document.getElementById('confirmAction').onclick = function() {
                deleteLocation(locationId);
                bootstrap.Modal.getInstance(document.getElementById('confirmationModal')).hide();
            };
            bootstrap.Modal.getInstance(document.getElementById('confirmationModal')).show();
        }
        
        function confirmDeleteSublocation(sublocationId) {
            document.getElementById('confirmationMessage').textContent = `Are you sure you want to delete sublocation #${sublocationId}?`;
            document.getElementById('confirmAction').onclick = function() {
                deleteSublocation(sublocationId);
                bootstrap.Modal.getInstance(document.getElementById('confirmationModal')).hide();
            };
            bootstrap.Modal.getInstance(document.getElementById('confirmationModal')).show();
        }
        
        // Delete functions (would be connected to API calls in a real app)
        function deletePackage(packageId) {
            console.log(`Deleting package ${packageId}`);
            // API call to delete package
            initializePackagesTable(); // Refresh table
        }
        
        function deleteProperty(propertyId) {
            console.log(`Deleting property ${propertyId}`);
            // API call to delete property
            initializePropertiesTable(); // Refresh table
        }
        
        function deleteLocation(locationId) {
            console.log(`Deleting location ${locationId}`);
            // API call to delete location
            initializeLocationsTable(); // Refresh table
        }
        
        function deleteSublocation(sublocationId) {
            console.log(`Deleting sublocation ${sublocationId}`);
            // API call to delete sublocation
            initializeSublocationsTable(); // Refresh table
        }
        
        // Other functions
        function toggleUserVerification(userId, isVerified) {
            console.log(`Toggling verification for user ${userId} to ${!isVerified}`);
            // API call to update verification status
            initializeUsersTable(); // Refresh table
        }
        
        function viewUserDetails(userId) {
            console.log(`Viewing details for user ${userId}`);
            // In a real app, you would show a modal or navigate to a user details page
            alert(`Viewing details for user ${userId}`);
        }
        
        function updatePaymentStatus(paymentId, status) {
            console.log(`Updating payment ${paymentId} to status ${status}`);
            // API call to update payment status
            initializePaymentsTable(); // Refresh table
        }
        
        function togglePropertyFeatured(propertyId, isFeatured) {
            console.log(`Setting property ${propertyId} featured status to ${isFeatured}`);
            // API call to update featured status
        }
        
        function togglePropertyVisibility(propertyId, isVisible) {
            console.log(`Setting property ${propertyId} visibility to ${isVisible}`);
            // API call to update visibility status
        }
        
        function updateInquiryStatus(inquiryId, status) {
            console.log(`Updating inquiry ${inquiryId} to status ${status}`);
            // API call to update inquiry status
            initializeInquiriesTable(); // Refresh table
        }
        
        function viewInquiryDetails(inquiryId) {
            console.log(`Viewing details for inquiry ${inquiryId}`);
            // In a real app, you would show a modal with the full inquiry details
            alert(`Viewing details for inquiry ${inquiryId}`);
        }
        
        function viewInteriorInquiry(inquiryId) {
            console.log(`Viewing interior design inquiry ${inquiryId}`);
            // In a real app, you would show a modal with the full inquiry details
            alert(`Viewing interior design inquiry ${inquiryId}`);
        }
        
        function deleteInteriorInquiry(inquiryId) {
            console.log(`Deleting interior design inquiry ${inquiryId}`);
            // API call to delete inquiry
            initializeInteriorTable(); // Refresh table
        }
        
        function addToTestimonials(inquiryId) {
            console.log(`Adding inquiry ${inquiryId} to testimonials`);
            // API call to add to testimonials
            alert(`Added inquiry ${inquiryId} to testimonials`);
        }
        
        function viewContactInquiry(inquiryId) {
            console.log(`Viewing contact inquiry ${inquiryId}`);
            // In a real app, you would show a modal with the full inquiry details
            alert(`Viewing contact inquiry ${inquiryId}`);
        }
        
        function deleteContactInquiry(inquiryId) {
            console.log(`Deleting contact inquiry ${inquiryId}`);
            // API call to delete inquiry
            initializeContactTable(); // Refresh table
        }
    