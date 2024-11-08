import React from "react";
import "./LoginSignup.css";
import { FaUser } from "react-icons/fa";
import { FaLock } from "react-icons/fa";

const LoginSignup = () => {
  return (
    <div className="container">
      <div className="header">
        <div className="text">Sign up</div>
        <div className="underline"></div>
      </div>
      <div className="inputs">
        <div className="input">
          <FaUser />
          <input type="text" name="" id="" />
        </div>
        <div className="input">
          <FaLock />
          <input type="password" name="" id="" />
        </div>
      </div>
      <div className="submit-container">
        <div className="submit">Sign Up</div>
        <div className="submit">Login</div>
      </div>
    </div>
  );
};

export default LoginSignup;
