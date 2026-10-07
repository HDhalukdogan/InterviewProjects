import React, { useEffect, useRef, useState } from 'react'
import agentAppointment from '../../lib/agentAppointment';

function Appointment() {

  const [appointments, setAppointments] = useState([])
  const [doctors, setDoctors] = useState([])
  const formRef = useRef()

  useEffect(() => {
    getAppointments();
    getDoctors();
  }, [])

  const getAppointments = async () => {
    try {
      const response = await agentAppointment.get('appointments');
      setAppointments(response.data);
    } catch (error) {
      console.error('Error fetching appointments:', error);
    }
  };

  const getDoctors = async () => {
    try {
      const response = await agentAppointment.get('doctors');
      setDoctors(response.data);
    } catch (error) {
      console.error('Error fetching doctors:', error);
    }
  };

  const createAppointment = async (e) => {
    e.preventDefault();
    const formData = new FormData(e.target);
    const appointmentData = {
      doctorId: formData.get('doctorId'),
      fullName: formData.get('fullName'),
      phone: formData.get('phone'),
      email: formData.get('email'),
      startTime: formData.get('startTime'),
      endTime: formData.get('endTime'),
    };
    try {
      const response = await agentAppointment.post('appointments', appointmentData);
      if (response.status === 204) {
        getAppointments();
        formRef.current.reset();
      }
    } catch (error) {
      console.error('Error creating appointment:', error?.response?.data);
    }
  };

  return (
    <div>
      <h1>Appointments</h1>
      <ul>
        {appointments.map((appointment) => (
          <li key={appointment.id}>
            {appointment.doctorName} - {appointment.patientName} - {new Date(appointment.startTime).toLocaleString()} - {new Date(appointment.endTime).toLocaleString()}
          </li>
        ))}
      </ul>
      <div>
        <h1>Create Appointment</h1>
        <form ref={formRef} onSubmit={createAppointment}>
          <div>
            <label>Doctor Name:</label>
            <select type="text" name="doctorId" >
              {doctors.map((doctor) => (
                <option key={doctor.id} value={doctor.id}>
                  {doctor.fullName}
                </option>
              ))}
            </select>
          </div>
          <div>
            <label>Patient Name:</label>
            <input type="text" name="fullName" />
          </div>
          <div>
            <label>Patient phone:</label>
            <input type="text" name="phone" />
          </div>
          <div>
            <label>Patient Email:</label>
            <input type="email" name="email" />
          </div>
          <div>
            <label>Start Time:</label>
            <input type="datetime-local" name="startTime" />
          </div>
          <div>
            <label>End Time:</label>
            <input type="datetime-local" name="endTime" />
          </div>
          <button type="submit">Create Appointment</button>
        </form>
      </div>
    </div>
  )
}

export default Appointment