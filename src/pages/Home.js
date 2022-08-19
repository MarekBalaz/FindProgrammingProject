import React from 'react';

export default function Home() {
  return (
    <div>
      <div>This will be the home page</div>
      <div>The following are links to their respective pages</div>
      <a href="/login">Login</a><br />
      <a href="/signup">SignUp</a><br />
      <a href="/forgotpassword">Forgot Password</a><br />
      <a href="/passwordreset">Password Reset</a><br />
    </div>
  )
}
