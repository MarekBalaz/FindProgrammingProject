import React from 'react'

export default function PasswordReset() {
  return (
    <main class='h-screen flex justify-center items-center'>
    <form class='flex flex-col justify-around'>
      <div class='flex flex-row justify-start items-center'>
        <p class="text-lg mb-0 mr-4">Set new password</p>
      </div>
      <hr class='mb-4 mt-2' /> 
      <input type="password" placeholder="New password" class='p-2 border-solid border-2 border-gray-300 rounded-md mb-4'/>
      <input type="password" placeholder="Confirm password" class='p-2 border-solid border-2 border-gray-300 rounded-md mb-4'/>
      <hr />
      <div>
        <input type='submit' value='Reset' class='px-4 py-2 my-4 bg-blue-600 rounded-md text-white hover:scale-110 hover:bg-blue-700 transition-all duration-300 cursor-pointer hover:shadow-lg'/>
      </div>
      <p class='text-xs bold'>Already have an account? <a href='SignIn.html' class='text-red-700'>Login</a></p>
    </form>
  </main>
  )
}
