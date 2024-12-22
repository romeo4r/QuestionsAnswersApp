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


// Event listener for opening the 'View Answers' modal
$('#viewAnswersModal').on('show.bs.modal', async function (event) {
    // Get the question ID from the button that triggered the modal
    var button = $(event.relatedTarget); // Button that triggered the modal
    var questionId = button.data('question-id'); // Extract the question ID

    // Fetch answers for the question from the API
    var response = await fetch(`https://localhost:5001/api/answer/question/${questionId}`);
    var answers = await response.json();

    // Clear any previous answers
    var answersList = $('#answersList');
    answersList.empty();

    // Display the answers
    if (answers.length > 0) {
        answers.forEach(function (answer) {
            answersList.append(`<li>${answer.Response} - <em>by ${answer.UserName}</em></li>`);
        });
    } else {
        answersList.append('<li>No answers yet.</li>');
    }
});
