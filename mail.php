<?php 
	$errors = array(); // array to hold validation errors

	$data = array(); // array to pass back data
	//echo $_POST['name'];
		$data['success'] = true;
		$data['messageSuccess'] = 'Hey! Gracias por contactarnos, en breve nos comunicaremos contigo';
		print_r($_POST);
		// CHANGE THE TWO LINES BELOW
		$email_from = $_POST['email']; // required
		$email_to = "shraddha@sparxitsolutions.com";
		$email_subject = "this is a test"; // required
		$name = $_POST['name']; // required
		$subject = $_POST['subject']; // required
		$message = $_POST['message']; // required
		
		// Form Message begin
		
		$email_message = "<table border='0' cellpadding='2' cellspacing='2' width='600'>
		<tr><td>Form details below:</td></tr>
		<tr><td>Name: ".$name." </td></tr>
		<tr><td>Email: ".$email_from."</td></tr>
		<tr><td>Subject: ".$subject."</td></tr>
		<tr><td>Message: ".$message."</td></tr>
		</table>";		

				
		// To send HTML mail, the Content-type header must be set
		$headers  = 'MIME-Version: 1.0' . "\r\n";
		$headers .= 'Content-type: text/html; charset=iso-8859-1' . "\r\n";
		
		// Additional headers
		$headers .= 'Reply-To: '.$email_from."\r\n";
		$headers .= 'From: '.$email_from . "\r\n";
		$headers .= 'Cc: '.$cc . "\r\n";
		$headers .= 'X-Mailer: PHP/' . phpversion();

		echo $email_to."====".$email_subject."------".$email_message.'--------'.$headers;
	// Send Mail
		
		if(@mail($email_to, $email_subject, $email_message, $headers))
		echo json_encode($data);

	// return all our data to an AJAX call
	
?>
