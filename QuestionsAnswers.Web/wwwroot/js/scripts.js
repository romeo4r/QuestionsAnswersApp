function logoutUser() {
    // Send a POST request to logout
    fetch('/Logout', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({}) // Optional: Add any body data if needed
    })
        .then(response => {
            if (response.ok) {
                // Redirect to Login page after successful logout
                window.location.href = '/Login';
            } else {
                alert('Logout failed!');
            }
        });
}

